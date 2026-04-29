using System.Collections.Generic;
using Entitas;
using UnityEngine;
using YuanCore.Core;

namespace YuanCore.Building;

/// <summary>
/// Placement 生命周期管理。
/// 负责建造/编辑中 Placement 的创建、取消恢复、提交确认、旋转。
/// 不包含输入采集逻辑——由各 System 调用。
/// </summary>
public static class PlacementLifecycle
{
    private static readonly List<Map.Entity> Buffer = [];

    // ═══════════════════════════════════════════════════
    //  建造模式：创建新 Placement
    // ═══════════════════════════════════════════════════

    /// <summary>
    /// 进入建造模式时调用，创建一个新建筑 Placement 实体。
    /// </summary>
    public static Map.Entity BeginNewPlacement(MapContext ctx, int buildingID, int taoZhuangID,
        BuildingRotation rotation)
    {
        var entity = ctx.CreateEntity();

        // 生成临时 UID
        var uid = $"_placement_{buildingID}";
        entity.AddBuilding(uid, buildingID);
        entity.AddBuildingState(taoZhuangID, rotation, false);
        entity.AddPlacement(Vector2Int.zero);

        // GridPosition 由 BuildingInputManager 在下一帧设定
        entity.AddGridPosition(CursorState.GridPosition);

        return entity;
    }

    // ═══════════════════════════════════════════════════
    //  编辑模式：已有建筑 → Placement
    // ═══════════════════════════════════════════════════

    /// <summary>
    /// 编辑选中建筑，进入 EditMove。
    /// 1. 从占用图临时移除旧建筑
    /// 2. 切换 View: Show → Placement
    /// 3. 挂 Placement / EditMoveSession 组件
    /// 4. 切换模式
    /// </summary>
    public static void BeginEditMove(MapContext ctx, string uid)
    {
        var entity = ctx.GetBuildingByUid(uid);
        if (entity == null)
        {
            YuanCorePlugin.Logger.LogWarning($"[PlacementLifecycle] Entity not found: {uid}");
            return;
        }

        var gridPos = entity.GetGridPosition().Value;
        var rotation = entity.GetBuildingState().Rotation;

        // 1. 从占用图移除
        BuildingManager.States.RemoveBuilding(uid);

        // 2. 挂编辑会话组件
        entity.AddBuildingRollback(gridPos, rotation);
        entity.AddPlacement(Vector2Int.zero);

        // 3. 请求视图切换 → PlacementView
        var state = entity.GetBuildingState();
        entity.ReplaceBuildingState(state.TaoZhuangID, state.Rotation, state.IsRuined);

        // 4. 兼容层同步
        MainloadCompat.SyncEditTarget(uid);

        // 5. 切换模式
        BuildingModeManager.SetMode(BuildingInteractionMode.EditMove);
    }

    // ═══════════════════════════════════════════════════
    //  取消所有当前会话的 Placement
    // ═══════════════════════════════════════════════════

    public static void CancelAllSessionPlacements(MapContext ctx)
    {
        CollectSessionPlacements(ctx);

        foreach (var entity in Buffer)
        {
            if (entity.HasBuildingRollback())
            {
                // 已有建筑——恢复原状
                var session = entity.GetBuildingRollback();

                // 恢复旋转
                if (entity.HasBuildingState())
                {
                    var bs = entity.GetBuildingState();
                    entity.ReplaceBuildingState(bs.TaoZhuangID, session.OriginalRotation, bs.IsRuined);
                }

                // 恢复位置
                entity.ReplaceGridPosition(session.OriginalGridPosition);

                // 恢复占用
                var building = entity.GetBuilding();
                BuildingManager.States.AddBuilding(
                    building.BuildingID, session.OriginalRotation,
                    session.OriginalGridPosition, building.Uid);

                // 清理组件
                entity.RemoveBuildingRollback();
                entity.RemovePlacement();

            }
            else
            {
                // 新建筑——直接销毁
                DestroyPlacementEntity(entity);
            }
        }
    }

    // ═══════════════════════════════════════════════════
    //  建造模式提交
    // ═══════════════════════════════════════════════════

    public static bool TrySubmitBuild(MapContext ctx)
    {
        CollectSessionPlacements(ctx);

        // 1. 复检所有 Placement
        if (!RecheckAll())
        {
            YuanCorePlugin.Logger.LogDebug("[PlacementLifecycle] Build recheck failed.");
            return false;
        }

        // 2. 业务可建造检查（简化：检查场景是否就绪）
        if (!MainloadCompat.IsSceneCreated)
        {
            YuanCorePlugin.Logger.LogDebug("[PlacementLifecycle] Scene not ready.");
            return false;
        }

        // 3. TODO: 扣除资源（需接入原版资源系统）
        //    BusinessBuildCheck.DeductResources(...)

        // 4. 提交每个 Placement
        foreach (var entity in Buffer)
        {
            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();
            var gridPos = entity.GetGridPosition().Value;

            // 分配正式 UID
            var newUid = "X000"; // TODO: 采用原版的递增UID
            entity.ReplaceBuilding(newUid, building.BuildingID);

            // 写入占用图
            BuildingManager.States.AddBuilding(building.BuildingID, state.Rotation, gridPos, newUid);

            // TODO: 写回原版建筑数据（SaveData / Mainload.BuildInto_x）
            //       SyncBuildingToVanilla(entity);

            // 清理 Placement 组件
            entity.RemovePlacement();

            // 切换 View
            entity.ReplaceBuildingState(state.TaoZhuangID, state.Rotation, state.IsRuined);
        }

        // 5. 退出建造模式（或可选继续放置）
        BuildingModeManager.SetMode(BuildingInteractionMode.Normal);
        MainloadCompat.SyncBuildPanelOpen(false);
        return true;
    }

    // ═══════════════════════════════════════════════════
    //  编辑移动提交
    // ═══════════════════════════════════════════════════

    public static bool TrySubmitEditMove(MapContext ctx)
    {
        CollectSessionPlacements(ctx);

        // 1. 复检
        if (!RecheckAll())
        {
            YuanCorePlugin.Logger.LogDebug("[PlacementLifecycle] EditMove recheck failed.");
            return false;
        }

        // 2. 提交
        foreach (var entity in Buffer)
        {
            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();
            var gridPos = entity.GetGridPosition().Value;

            // 写入新占用
            BuildingManager.States.AddBuilding(
                building.BuildingID, state.Rotation, gridPos, building.Uid);

            // TODO: 写回原版数据中的位置/旋转
            //       SyncBuildingPositionToVanilla(entity);

            // 清理组件
            entity.RemoveBuildingRollback();
            entity.RemovePlacement();

            // 切换回 ShowView
            entity.ReplaceBuildingState(state.TaoZhuangID, state.Rotation, state.IsRuined);

            // 触发 LinkMaterial 更新
            entity.AddLinkMaterialUpdate(1);
        }

        // 回到 EditSelect（或 Normal，取决于交互设计）
        BuildingModeManager.SetMode(BuildingInteractionMode.EditSelect);
        MainloadCompat.SyncEditTarget("null");
        return true;
    }

    // ═══════════════════════════════════════════════════
    //  旋转
    // ═══════════════════════════════════════════════════

    public static void RotateSessionPlacements(MapContext ctx)
    {
        CollectSessionPlacements(ctx);

        foreach (var entity in Buffer)
        {
            if (!entity.HasBuildingState()) continue;
            var bs = entity.GetBuildingState();
            var newRot = (BuildingRotation)(((int)bs.Rotation + 1) % 4);

            // 检查该旋转是否在 ShapeRegistry 中注册
            var bid = entity.GetBuilding().BuildingID;
            if (!BuildingShapeRegistry.Contains(bid, newRot))
            {
                YuanCorePlugin.Logger.LogDebug(
                    $"[PlacementLifecycle] Rotation {newRot} not registered for {bid}, skipping.");
                continue;
            }

            entity.ReplaceBuildingState(bs.TaoZhuangID, newRot, bs.IsRuined);
            // PlacementValidationSystem 会在下一帧重新检测
            // CreateViewSystem 会在 BuildingState 替换时重建 Placement 视图
        }
    }

    // ═══════════════════════════════════════════════════
    //  内部工具
    // ═══════════════════════════════════════════════════

    private static void CollectSessionPlacements(MapContext ctx)
    {
        Buffer.Clear();
        var group = ctx.GetGroup(
            Matcher<Map.Entity>.AllOf(
                YuanCoreBuildingMapPlacementMatcher.Placement));
        foreach (var entity in group.GetEntities())
            Buffer.Add(entity);
    }

    /// <summary>
    /// 对缓冲区内所有 Placement 执行完整复检。
    /// </summary>
    private static bool RecheckAll()
    {
        foreach (var entity in Buffer)
        {
            if (!entity.HasBuilding() || !entity.HasBuildingState() || !entity.HasGridPosition())
                return false;

            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();
            var gridPos = entity.GetGridPosition().Value;

            if (!BuildingManager.States.CheckCanBuild(
                    building.BuildingID, state.Rotation, gridPos, out _))
                return false;
        }
        return true;
    }

    private static void DestroyPlacementEntity(Map.Entity entity)
    {
        // 先销毁 View GameObject
        if (entity.HasView())
        {
            var view = entity.GetView().View;
            if (view is MonoBehaviour mb && mb != null)
                Object.Destroy(mb.gameObject);
            entity.RemoveView();
        }

        entity.Destroy();
    }
}
