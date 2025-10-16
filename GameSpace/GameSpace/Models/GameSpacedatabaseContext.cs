using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Models;

public partial class GameSpacedatabaseContext : DbContext
{
    public GameSpacedatabaseContext(DbContextOptions<GameSpacedatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BannedWord> BannedWords { get; set; }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CouponType> CouponTypes { get; set; }

    public virtual DbSet<CsAgent> CsAgents { get; set; }

    public virtual DbSet<CsAgentPermission> CsAgentPermissions { get; set; }

    public virtual DbSet<DmConversation> DmConversations { get; set; }

    public virtual DbSet<DmMessage> DmMessages { get; set; }

    public virtual DbSet<Evoucher> Evouchers { get; set; }

    public virtual DbSet<EvoucherRedeemLog> EvoucherRedeemLogs { get; set; }

    public virtual DbSet<EvoucherToken> EvoucherTokens { get; set; }

    public virtual DbSet<EvoucherType> EvoucherTypes { get; set; }

    public virtual DbSet<Forum> Forums { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameMetricDaily> GameMetricDailies { get; set; }

    public virtual DbSet<GameProductDetail> GameProductDetails { get; set; }

    public virtual DbSet<GameSourceMap> GameSourceMaps { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupBlock> GroupBlocks { get; set; }

    public virtual DbSet<GroupChat> GroupChats { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<GroupReadState> GroupReadStates { get; set; }

    public virtual DbSet<LeaderboardSnapshot> LeaderboardSnapshots { get; set; }

    public virtual DbSet<ManagerDatum> ManagerData { get; set; }

    public virtual DbSet<ManagerRolePermission> ManagerRolePermissions { get; set; }

    public virtual DbSet<MemberSalesProfile> MemberSalesProfiles { get; set; }

    public virtual DbSet<MerchType> MerchTypes { get; set; }

    public virtual DbSet<Metric> Metrics { get; set; }

    public virtual DbSet<MetricSource> MetricSources { get; set; }

    public virtual DbSet<MiniGame> MiniGames { get; set; }

    public virtual DbSet<Mute> Mutes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationAction> NotificationActions { get; set; }

    public virtual DbSet<NotificationRecipient> NotificationRecipients { get; set; }

    public virtual DbSet<NotificationSource> NotificationSources { get; set; }

    public virtual DbSet<OfficialStoreRanking> OfficialStoreRankings { get; set; }

    public virtual DbSet<OrderAddress> OrderAddresses { get; set; }

    public virtual DbSet<OrderInfo> OrderInfos { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<OtherProductDetail> OtherProductDetails { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<PlayerMarketOrderInfo> PlayerMarketOrderInfos { get; set; }

    public virtual DbSet<PlayerMarketOrderTradepage> PlayerMarketOrderTradepages { get; set; }

    public virtual DbSet<PlayerMarketProductImg> PlayerMarketProductImgs { get; set; }

    public virtual DbSet<PlayerMarketProductInfo> PlayerMarketProductInfos { get; set; }

    public virtual DbSet<PlayerMarketRanking> PlayerMarketRankings { get; set; }

    public virtual DbSet<PlayerMarketTradeMsg> PlayerMarketTradeMsgs { get; set; }

    public virtual DbSet<PopularityIndexDaily> PopularityIndexDailies { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostMetricSnapshot> PostMetricSnapshots { get; set; }

    public virtual DbSet<PostSource> PostSources { get; set; }

    public virtual DbSet<ProductCode> ProductCodes { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductInfo> ProductInfos { get; set; }

    public virtual DbSet<ProductInfoAuditLog> ProductInfoAuditLogs { get; set; }

    public virtual DbSet<Reaction> Reactions { get; set; }

    public virtual DbSet<Relation> Relations { get; set; }

    public virtual DbSet<RelationStatus> RelationStatuses { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<StockMovement> StockMovements { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    public virtual DbSet<SupportTicketAssignment> SupportTicketAssignments { get; set; }

    public virtual DbSet<SupportTicketMessage> SupportTicketMessages { get; set; }

    public virtual DbSet<Thread> Threads { get; set; }

    public virtual DbSet<ThreadPost> ThreadPosts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserIntroduce> UserIntroduces { get; set; }

    public virtual DbSet<UserRight> UserRights { get; set; }

    public virtual DbSet<UserSalesInformation> UserSalesInformations { get; set; }

    public virtual DbSet<UserSignInStat> UserSignInStats { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<UserWallet> UserWallets { get; set; }

    public virtual DbSet<VCsEligibleAgent> VCsEligibleAgents { get; set; }

    public virtual DbSet<WalletHistory> WalletHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BannedWord>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PK__banned_w__7FFA1D406FBDDC61");

            entity.ToTable("banned_words");

            entity.Property(e => e.WordId).HasColumnName("word_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Word)
                .HasMaxLength(50)
                .HasColumnName("word");
        });

        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__bookmark__3213E83F2689F12E");

            entity.ToTable("bookmarks");

            entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId }, "bookmarks_index_20").IsUnique();

            entity.HasIndex(e => new { e.TargetType, e.TargetId }, "bookmarks_index_21");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.TargetId).HasColumnName("target_id");
            entity.Property(e => e.TargetType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("target_type");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__bookmarks__User___6BAEFA67");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PK_Coupon");

            entity.ToTable("Coupon");

            entity.HasIndex(e => new { e.UserId, e.IsUsed, e.CouponId }, "Coupon_index_26");

            entity.Property(e => e.CouponCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CouponId).HasColumnName("CouponID");
            entity.Property(e => e.CouponTypeId).HasColumnName("CouponTypeID");
            entity.Property(e => e.UsedInOrderId).HasColumnName("UsedInOrderID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.UsedInOrder).WithMany()
                .HasForeignKey(d => d.UsedInOrderId)
                .HasConstraintName("FK__Coupon__UsedInOr__6991A7CB");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Coupon__UserID__689D8392");
        });

        modelBuilder.Entity<CouponType>(entity =>
        {
            entity.HasKey(e => e.CouponTypeId).HasName("PK_CouponType");

            entity.ToTable("CouponType");

            entity.Property(e => e.CouponTypeId).HasColumnName("CouponTypeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountType).HasMaxLength(10);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MinSpend).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<CsAgent>(entity =>
        {
            entity.HasKey(e => e.AgentId);

            entity.ToTable("CS_Agent", tb => tb.HasTrigger("trg_CS_Agent_EnsureEligible"));

            entity.HasIndex(e => e.IsActive, "IX_CS_Agent_IsActive");

            entity.HasIndex(e => e.ManagerId, "UQ_CS_Agent_Manager").IsUnique();

            entity.Property(e => e.AgentId).HasColumnName("agent_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByManager).HasColumnName("created_by_manager");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.MaxConcurrent)
                .HasDefaultValue((byte)5)
                .HasColumnName("max_concurrent");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByManager).HasColumnName("updated_by_manager");

            entity.HasOne(d => d.CreatedByManagerNavigation).WithMany(p => p.CsAgentCreatedByManagerNavigations)
                .HasForeignKey(d => d.CreatedByManager)
                .HasConstraintName("FK_CS_Agent_CreatedBy");

            entity.HasOne(d => d.Manager).WithOne(p => p.CsAgentManager)
                .HasForeignKey<CsAgent>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CS_Agent_Manager");

            entity.HasOne(d => d.UpdatedByManagerNavigation).WithMany(p => p.CsAgentUpdatedByManagerNavigations)
                .HasForeignKey(d => d.UpdatedByManager)
                .HasConstraintName("FK_CS_Agent_UpdatedBy");
        });

        modelBuilder.Entity<CsAgentPermission>(entity =>
        {
            entity.HasKey(e => e.AgentPermissionId);

            entity.ToTable("CS_Agent_Permission");

            entity.HasIndex(e => e.CanAssign, "IX_CS_Perm_CanAssign");

            entity.HasIndex(e => e.CanEditMuteAll, "IX_CS_Perm_EditAll");

            entity.HasIndex(e => e.AgentId, "UQ_CS_Perm_Agent").IsUnique();

            entity.Property(e => e.AgentPermissionId).HasColumnName("agent_permission_id");
            entity.Property(e => e.AgentId).HasColumnName("agent_id");
            entity.Property(e => e.CanAccept).HasColumnName("can_accept");
            entity.Property(e => e.CanAssign).HasColumnName("can_assign");
            entity.Property(e => e.CanEditMuteAll).HasColumnName("can_edit_mute_all");
            entity.Property(e => e.CanTransfer).HasColumnName("can_transfer");

            entity.HasOne(d => d.Agent).WithOne(p => p.CsAgentPermission)
                .HasForeignKey<CsAgentPermission>(d => d.AgentId)
                .HasConstraintName("FK_CS_Perm_Agent");
        });

        modelBuilder.Entity<DmConversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId);

            entity.ToTable("DM_Conversations", tb => tb.HasTrigger("TR_DMC_ValidateEndpoints"));

            entity.HasIndex(e => new { e.IsManagerDm, e.Party1Id, e.LastMessageAt }, "IX_DMC_List_ByParty1").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.IsManagerDm, e.Party2Id, e.LastMessageAt }, "IX_DMC_List_ByParty2").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.IsManagerDm, e.Party1Id, e.Party2Id }, "UQ_DM_Conversations_Pair").IsUnique();

            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsManagerDm).HasColumnName("is_manager_dm");
            entity.Property(e => e.LastMessageAt).HasColumnName("last_message_at");
            entity.Property(e => e.Party1Id).HasColumnName("party1_id");
            entity.Property(e => e.Party2Id).HasColumnName("party2_id");
        });

        modelBuilder.Entity<DmMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("DM_Messages", tb =>
                {
                    tb.HasTrigger("TR_DMM_AfterInsert_UpdateConversation");
                    tb.HasTrigger("TR_DMM_InsteadOfUpdate_ReadFlag");
                });

            entity.HasIndex(e => new { e.ConversationId, e.EditedAt }, "IX_Msg_ConvTime");

            entity.HasIndex(e => e.ConversationId, "IX_Msg_Unread").HasFilter("([is_read]=(0))");

            entity.HasIndex(e => new { e.ConversationId, e.EditedAt }, "IX_Msg_Unread_ForParty1").HasFilter("([is_read]=(0) AND [sender_is_party1]=(0))");

            entity.HasIndex(e => new { e.ConversationId, e.EditedAt }, "IX_Msg_Unread_ForParty2").HasFilter("([is_read]=(0) AND [sender_is_party1]=(1))");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.EditedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("edited_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.MessageText)
                .HasMaxLength(255)
                .HasColumnName("message_text");
            entity.Property(e => e.ReadAt).HasColumnName("read_at");
            entity.Property(e => e.SenderIsParty1).HasColumnName("sender_is_party1");

            entity.HasOne(d => d.Conversation).WithMany(p => p.DmMessages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("FK_DM_Messages_Conversation");
        });

        modelBuilder.Entity<Evoucher>(entity =>
        {
            entity.HasKey(e => e.EvoucherId).HasName("PK_EVoucher");

            entity.ToTable("EVoucher");

            entity.HasIndex(e => new { e.UserId, e.IsUsed, e.EvoucherId }, "EVoucher_index_27");

            entity.Property(e => e.EvoucherCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EVoucherCode");
            entity.Property(e => e.EvoucherId).HasColumnName("EVoucherID");
            entity.Property(e => e.EvoucherTypeId).HasColumnName("EVoucherTypeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EVoucher__UserID__6B79F03D");
        });

        modelBuilder.Entity<EvoucherRedeemLog>(entity =>
        {
            entity.HasKey(e => e.RedeemId).HasName("PK_EVoucherRedeemLog");

            entity.ToTable("EVoucherRedeemLog");

            entity.HasIndex(e => new { e.EvoucherId, e.ScannedAt }, "EVoucherRedeemLog_index_28");

            entity.HasIndex(e => new { e.UserId, e.ScannedAt }, "EVoucherRedeemLog_index_29");

            entity.Property(e => e.EvoucherId).HasColumnName("EVoucherID");
            entity.Property(e => e.RedeemId).HasColumnName("RedeemID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TokenId).HasColumnName("TokenID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EVoucherR__UserI__61F08603");
        });

        modelBuilder.Entity<EvoucherToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK_EVoucherToken");

            entity.ToTable("EVoucherToken");

            entity.Property(e => e.EvoucherId).HasColumnName("EVoucherID");
            entity.Property(e => e.Token)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.TokenId).HasColumnName("TokenID");
        });

        modelBuilder.Entity<EvoucherType>(entity =>
        {
            entity.HasKey(e => e.EvoucherTypeId).HasName("PK_EVoucherType");

            entity.ToTable("EVoucherType");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EvoucherTypeId).HasColumnName("EVoucherTypeID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ValueAmount).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Forum>(entity =>
        {
            entity.HasKey(e => e.ForumId).HasName("PK__forums__69A2FA58CFFF3479");

            entity.ToTable("forums");

            entity.HasIndex(e => e.GameId, "forums_index_15").IsUnique();

            entity.Property(e => e.ForumId).HasColumnName("forum_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.Game).WithOne(p => p.Forum)
                .HasForeignKey<Forum>(d => d.GameId)
                .HasConstraintName("FK__forums__game_id__6501FCD8");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__games__FFE11FCFDAD0C969");

            entity.ToTable("games");

            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Genre)
                .HasMaxLength(50)
                .HasColumnName("genre");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.NameZh)
                .HasMaxLength(100)
                .HasColumnName("name_zh");
        });

        modelBuilder.Entity<GameMetricDaily>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__game_met__3213E83F3E2670CA");

            entity.ToTable("game_metric_daily");

            entity.HasIndex(e => new { e.GameId, e.MetricId, e.Date }, "game_metric_daily_index_3").IsUnique();

            entity.HasIndex(e => new { e.Date, e.MetricId }, "game_metric_daily_index_4");

            entity.HasIndex(e => new { e.GameId, e.Date }, "game_metric_daily_index_5");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AggMethod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("agg_method");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.MetricId).HasColumnName("metric_id");
            entity.Property(e => e.Quality)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("quality");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.Value)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("value");

            entity.HasOne(d => d.Game).WithMany(p => p.GameMetricDailies)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__game_metr__game___09FE775D");

            entity.HasOne(d => d.Metric).WithMany(p => p.GameMetricDailies)
                .HasForeignKey(d => d.MetricId)
                .HasConstraintName("FK__game_metr__metri__0AF29B96");
        });

        modelBuilder.Entity<GameProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__GameProd__47027DF5FC0CBD29");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.DownloadLink)
                .HasMaxLength(500)
                .HasColumnName("download_link");
            entity.Property(e => e.GameType)
                .HasMaxLength(200)
                .HasColumnName("game_type");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PlatformId).HasColumnName("platform_id");
            entity.Property(e => e.PlatformName)
                .HasMaxLength(100)
                .HasColumnName("platform_name");
            entity.Property(e => e.ProductDescription).HasColumnName("product_description");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

            entity.HasOne(d => d.Product).WithOne(p => p.GameProductDetail)
                .HasForeignKey<GameProductDetail>(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameProductDetails_ProductInfo");

            entity.HasOne(d => d.Supplier).WithMany(p => p.GameProductDetails)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameProductDetails_Supplier");
        });

        modelBuilder.Entity<GameSourceMap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__game_sou__3213E83F28A4459D");

            entity.ToTable("game_source_map");

            entity.HasIndex(e => new { e.GameId, e.SourceId }, "game_source_map_index_1").IsUnique();

            entity.HasIndex(e => new { e.SourceId, e.ExternalKey }, "game_source_map_index_2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ExternalKey)
                .HasMaxLength(255)
                .HasColumnName("external_key");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.SourceId).HasColumnName("source_id");

            entity.HasOne(d => d.Game).WithMany(p => p.GameSourceMaps)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__game_sour__game___08162EEB");

            entity.HasOne(d => d.Source).WithMany(p => p.GameSourceMaps)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK__game_sour__sourc__090A5324");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasIndex(e => e.GroupName, "IX_Groups_Name");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasColumnName("group_name");
            entity.Property(e => e.IsPrivate).HasColumnName("is_private");
            entity.Property(e => e.OwnerUserId).HasColumnName("owner_user_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.Groups)
                .HasForeignKey(d => d.OwnerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_Owner");
        });

        modelBuilder.Entity<GroupBlock>(entity =>
        {
            entity.HasKey(e => e.BlockId);

            entity.ToTable("Group_Block");

            entity.HasIndex(e => new { e.GroupId, e.UserId }, "UQ_Group_Block_Current")
                .IsUnique()
                .HasFilter("([unblocked_at] IS NULL)");

            entity.Property(e => e.BlockId).HasColumnName("block_id");
            entity.Property(e => e.BlockedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("blocked_at");
            entity.Property(e => e.BlockedByUserId).HasColumnName("blocked_by_user_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.UnblockedAt).HasColumnName("unblocked_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.BlockedByUser).WithMany(p => p.GroupBlockBlockedByUsers)
                .HasForeignKey(d => d.BlockedByUserId)
                .HasConstraintName("FK_Group_Block_ByUser");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupBlocks)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Group_Block_Group");

            entity.HasOne(d => d.User).WithMany(p => p.GroupBlockUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_Block_User");
        });

        modelBuilder.Entity<GroupChat>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("Group_Chat");

            entity.HasIndex(e => new { e.GroupId, e.SentAt }, "IX_Group_Chat_Group_SentAt");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.MessageText).HasColumnName("message_text");
            entity.Property(e => e.SenderUserId).HasColumnName("sender_user_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupChats)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Group_Chat_Group");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.GroupChats)
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_Chat_Sender");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.ToTable("Group_Member");

            entity.HasIndex(e => new { e.GroupId, e.UserId }, "UQ_Group_Member_Current")
                .IsUnique()
                .HasFilter("([left_at] IS NULL)");

            entity.Property(e => e.GroupMemberId).HasColumnName("group_member_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("joined_at");
            entity.Property(e => e.LeftAt).HasColumnName("left_at");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasDefaultValue("member")
                .HasColumnName("role_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Group_Member_Group");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_Member_User");
        });

        modelBuilder.Entity<GroupReadState>(entity =>
        {
            entity.HasKey(e => e.StateId);

            entity.ToTable("Group_Read_States");

            entity.HasIndex(e => new { e.GroupId, e.UserId }, "UQ_Group_Read_States_GroupUser").IsUnique();

            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.LastReadAt).HasColumnName("last_read_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupReadStates)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Group_Read_States_Group");

            entity.HasOne(d => d.User).WithMany(p => p.GroupReadStates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_Read_States_User");
        });

        modelBuilder.Entity<LeaderboardSnapshot>(entity =>
        {
            entity.HasKey(e => e.SnapshotId).HasName("PK__leaderbo__C27CFBF749AA0C32");

            entity.ToTable("leaderboard_snapshots");

            entity.HasIndex(e => new { e.Period, e.Ts, e.GameId }, "leaderboard_snapshots_index_10");

            entity.HasIndex(e => new { e.Period, e.Ts, e.Rank }, "leaderboard_snapshots_index_8");

            entity.HasIndex(e => new { e.Period, e.Ts, e.Rank, e.GameId }, "leaderboard_snapshots_index_9").IsUnique();

            entity.Property(e => e.SnapshotId).HasColumnName("snapshot_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.IndexValue)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("index_value");
            entity.Property(e => e.Period)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("period");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.Ts).HasColumnName("ts");

            entity.HasOne(d => d.Game).WithMany(p => p.LeaderboardSnapshots)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__leaderboa__game___062DE679");
        });

        modelBuilder.Entity<ManagerDatum>(entity =>
        {
            entity.HasKey(e => e.ManagerId).HasName("PK__ManagerD__AE5FEFAD638D88FF");

            entity.HasIndex(e => e.ManagerEmail, "UQ__ManagerD__0890969EC9C76047").IsUnique();

            entity.HasIndex(e => e.ManagerAccount, "UQ__ManagerD__62B5E21119A93877").IsUnique();

            entity.Property(e => e.ManagerId)
                .ValueGeneratedNever()
                .HasColumnName("Manager_Id");
            entity.Property(e => e.AdministratorRegistrationDate).HasColumnName("Administrator_registration_date");
            entity.Property(e => e.ManagerAccessFailedCount).HasColumnName("Manager_AccessFailedCount");
            entity.Property(e => e.ManagerAccount)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Manager_Account");
            entity.Property(e => e.ManagerEmail)
                .HasMaxLength(255)
                .HasColumnName("Manager_Email");
            entity.Property(e => e.ManagerEmailConfirmed).HasColumnName("Manager_EmailConfirmed");
            entity.Property(e => e.ManagerLockoutEnabled)
                .HasDefaultValue(true)
                .HasColumnName("Manager_LockoutEnabled");
            entity.Property(e => e.ManagerLockoutEnd).HasColumnName("Manager_LockoutEnd");
            entity.Property(e => e.ManagerName)
                .HasMaxLength(30)
                .HasColumnName("Manager_Name");
            entity.Property(e => e.ManagerPassword)
                .HasMaxLength(200)
                .HasColumnName("Manager_Password");

            entity.HasMany(d => d.ManagerRoles).WithMany(p => p.Managers)
                .UsingEntity<Dictionary<string, object>>(
                    "ManagerRole",
                    r => r.HasOne<ManagerRolePermission>().WithMany()
                        .HasForeignKey("ManagerRoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ManagerRo__Manag__0CDAE408"),
                    l => l.HasOne<ManagerDatum>().WithMany()
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ManagerRo__Manag__0BE6BFCF"),
                    j =>
                    {
                        j.HasKey("ManagerId", "ManagerRoleId").HasName("PK__ManagerR__6270897EA52FCCCF");
                        j.ToTable("ManagerRole");
                        j.IndexerProperty<int>("ManagerId").HasColumnName("Manager_Id");
                        j.IndexerProperty<int>("ManagerRoleId").HasColumnName("ManagerRole_Id");
                    });
        });

        modelBuilder.Entity<ManagerRolePermission>(entity =>
        {
            entity.HasKey(e => e.ManagerRoleId).HasName("PK__ManagerR__C2F66D3DC40C7408");

            entity.ToTable("ManagerRolePermission");

            entity.Property(e => e.ManagerRoleId)
                .ValueGeneratedNever()
                .HasColumnName("ManagerRole_Id");
            entity.Property(e => e.CustomerService).HasColumnName("customer_service");
            entity.Property(e => e.PetRightsManagement).HasColumnName("Pet_Rights_Management");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<MemberSalesProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__MemberSa__206D9170B9BF5CEA");

            entity.ToTable("MemberSalesProfile");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("User_Id");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.MemberSalesProfile)
                .HasForeignKey<MemberSalesProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MemberSal__User___119F9925");
        });

        modelBuilder.Entity<MerchType>(entity =>
        {
            entity.HasKey(e => e.MerchTypeId).HasName("PK__MerchTyp__894B2F3111B26F3C");

            entity.ToTable("MerchType");

            entity.Property(e => e.MerchTypeId).HasColumnName("merch_type_id");
            entity.Property(e => e.MerchTypeName)
                .HasMaxLength(50)
                .HasColumnName("merch_type_name");
        });

        modelBuilder.Entity<Metric>(entity =>
        {
            entity.HasKey(e => e.MetricId).HasName("PK__metrics__13D5DCA4360F643C");

            entity.ToTable("metrics");

            entity.HasIndex(e => new { e.SourceId, e.Code }, "metrics_index_0").IsUnique();

            entity.Property(e => e.MetricId).HasColumnName("metric_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.SourceId).HasColumnName("source_id");
            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("unit");

            entity.HasOne(d => d.Source).WithMany(p => p.Metrics)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK__metrics__source___0DCF0841");
        });

        modelBuilder.Entity<MetricSource>(entity =>
        {
            entity.HasKey(e => e.SourceId).HasName("PK__metric_s__3035A9B60C38D610");

            entity.ToTable("metric_sources");

            entity.Property(e => e.SourceId).HasColumnName("source_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Note)
                .HasMaxLength(50)
                .HasColumnName("note");
        });

        modelBuilder.Entity<MiniGame>(entity =>
        {
            entity.HasKey(e => e.PlayId).HasName("PK_MiniGame");

            entity.ToTable("MiniGame");

            entity.HasIndex(e => new { e.UserId, e.StartTime }, "MiniGame_index_24");

            entity.HasIndex(e => new { e.PetId, e.StartTime }, "MiniGame_index_25");

            entity.Property(e => e.CouponGained)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PetId).HasColumnName("PetID");
            entity.Property(e => e.PlayId).HasColumnName("PlayID");
            entity.Property(e => e.Result).HasMaxLength(10);
            entity.Property(e => e.SpeedMultiplier).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MiniGame__UserID__6F7F8B4B");
        });

        modelBuilder.Entity<Mute>(entity =>
        {
            entity.HasIndex(e => e.Word, "UQ_Mutes_Word").IsUnique();

            entity.Property(e => e.MuteId).HasColumnName("mute_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Replacement)
                .HasMaxLength(50)
                .HasColumnName("replacement");
            entity.Property(e => e.Word)
                .HasMaxLength(50)
                .HasColumnName("word");

            entity.HasOne(d => d.Manager).WithMany(p => p.Mutes)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_Mutes_Manager");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => new { e.GroupId, e.CreatedAt }, "IX_Notifications_Group_Created").IsDescending(false, true);

            entity.HasIndex(e => new { e.SourceId, e.ActionId, e.CreatedAt }, "IX_Notifications_Source_Action_Created").IsDescending(false, false, true);

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.ActionId).HasColumnName("action_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .HasColumnName("message");
            entity.Property(e => e.SenderManagerId).HasColumnName("sender_manager_id");
            entity.Property(e => e.SenderUserId).HasColumnName("sender_user_id");
            entity.Property(e => e.SourceId).HasColumnName("source_id");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.Action).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Action");

            entity.HasOne(d => d.Group).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Notifications_Group");

            entity.HasOne(d => d.SenderManager).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.SenderManagerId)
                .HasConstraintName("FK_Notifications_SenderManager");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.SenderUserId)
                .HasConstraintName("FK_Notifications_SenderUser");

            entity.HasOne(d => d.Source).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.SourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Source");
        });

        modelBuilder.Entity<NotificationAction>(entity =>
        {
            entity.HasKey(e => e.ActionId);

            entity.ToTable("Notification_Actions");

            entity.HasIndex(e => e.ActionName, "UQ_Notification_Actions_Name").IsUnique();

            entity.Property(e => e.ActionId).HasColumnName("action_id");
            entity.Property(e => e.ActionName)
                .HasMaxLength(50)
                .HasColumnName("action_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<NotificationRecipient>(entity =>
        {
            entity.HasKey(e => e.RecipientId);

            entity.ToTable("Notification_Recipients");

            entity.HasIndex(e => new { e.ManagerId, e.ReadAt, e.NotificationId }, "IX_Recipients_Manager_Read")
                .IsDescending(false, false, true)
                .HasFilter("([manager_id] IS NOT NULL)");

            entity.HasIndex(e => new { e.UserId, e.ReadAt, e.NotificationId }, "IX_Recipients_User_Read")
                .IsDescending(false, false, true)
                .HasFilter("([user_id] IS NOT NULL)");

            entity.HasIndex(e => new { e.NotificationId, e.ManagerId }, "UQ_Recipients_Notification_Manager")
                .IsUnique()
                .HasFilter("([manager_id] IS NOT NULL)");

            entity.HasIndex(e => new { e.NotificationId, e.UserId }, "UQ_Recipients_Notification_User")
                .IsUnique()
                .HasFilter("([user_id] IS NOT NULL)");

            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.ReadAt).HasColumnName("read_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.NotificationRecipients)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_Recipients_Manager");

            entity.HasOne(d => d.Notification).WithMany(p => p.NotificationRecipients)
                .HasForeignKey(d => d.NotificationId)
                .HasConstraintName("FK_Recipients_Notification");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationRecipients)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Recipients_User");
        });

        modelBuilder.Entity<NotificationSource>(entity =>
        {
            entity.HasKey(e => e.SourceId);

            entity.ToTable("Notification_Sources");

            entity.HasIndex(e => e.SourceName, "UQ_Notification_Sources_Name").IsUnique();

            entity.Property(e => e.SourceId).HasColumnName("source_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.SourceName)
                .HasMaxLength(50)
                .HasColumnName("source_name");
        });

        modelBuilder.Entity<OfficialStoreRanking>(entity =>
        {
            entity.HasKey(e => e.RankingId).HasName("PK__Official__95F5B23DDE60FACA");

            entity.ToTable("Official_Store_Ranking");

            entity.Property(e => e.RankingId).HasColumnName("ranking_id");
            entity.Property(e => e.PeriodType)
                .HasMaxLength(20)
                .HasColumnName("period_type");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.RankingDate).HasColumnName("ranking_date");
            entity.Property(e => e.RankingMetric)
                .HasMaxLength(50)
                .HasColumnName("ranking_metric");
            entity.Property(e => e.RankingPosition).HasColumnName("ranking_position");
            entity.Property(e => e.RankingUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("ranking_updated_at");
            entity.Property(e => e.TradingAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("trading_amount");
            entity.Property(e => e.TradingVolume).HasColumnName("trading_volume");
        });

        modelBuilder.Entity<OrderAddress>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__OrderAdd__465962297AF1A4A5");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.Address1)
                .HasMaxLength(200)
                .HasColumnName("address1");
            entity.Property(e => e.Address2)
                .HasMaxLength(200)
                .HasColumnName("address2");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(30)
                .HasColumnName("country");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.Recipient)
                .HasMaxLength(100)
                .HasColumnName("recipient");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(10)
                .HasColumnName("zipcode");
        });

        modelBuilder.Entity<OrderInfo>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__OrderInf__46596229C3B9E39E");

            entity.ToTable("OrderInfo");

            entity.HasIndex(e => new { e.OrderDate, e.OrderCode }, "IX_OrderInfo_OrderDate_OrderCode");

            entity.HasIndex(e => new { e.PaymentStatus, e.OrderDate }, "IX_OrderInfo_PaymentStatus");

            entity.HasIndex(e => new { e.OrderStatus, e.OrderDate }, "IX_OrderInfo_Status_Date");

            entity.HasIndex(e => new { e.UserId, e.OrderDate }, "IX_OrderInfo_User_Date");

            entity.HasIndex(e => e.OrderCode, "UQ_OrderInfo_OrderCode").IsUnique();

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.OrderCode)
                .HasDefaultValueSql("(NEXT VALUE FOR [dbo].[SeqOrderCode])")
                .HasColumnName("order_code");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("order_date");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(30)
                .HasDefaultValue("未出貨")
                .HasColumnName("order_status");
            entity.Property(e => e.OrderTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("order_total");
            entity.Property(e => e.PaymentAt).HasColumnName("payment_at");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(30)
                .HasDefaultValue("未付款")
                .HasColumnName("payment_status");
            entity.Property(e => e.ShippedAt).HasColumnName("shipped_at");
            entity.Property(e => e.UserId).HasColumnName("User_ID");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__OrderIte__52020FDD2D390A7A");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.LineNo).HasColumnName("line_no");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("(CONVERT([decimal](18,2),[unit_price]*[quantity]))", true)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__order__5A4F643B");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__OrderSta__096AA2E9A89BA70E");

            entity.ToTable("OrderStatusHistory");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.FromStatus)
                .HasMaxLength(30)
                .HasColumnName("from_status");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ToStatus)
                .HasMaxLength(30)
                .HasColumnName("to_status");
        });

        modelBuilder.Entity<OtherProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__OtherPro__47027DF5D4B3F204");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.DigitalCode)
                .HasMaxLength(100)
                .HasColumnName("digital_code");
            entity.Property(e => e.Dimensions)
                .HasMaxLength(100)
                .HasColumnName("dimensions");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Material)
                .HasMaxLength(50)
                .HasColumnName("material");
            entity.Property(e => e.MerchTypeId).HasColumnName("merch_type_id");
            entity.Property(e => e.ProductDescription).HasColumnName("product_description");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .HasColumnName("size");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Weight)
                .HasMaxLength(50)
                .HasColumnName("weight");

            entity.HasOne(d => d.MerchType).WithMany(p => p.OtherProductDetails)
                .HasForeignKey(d => d.MerchTypeId)
                .HasConstraintName("FK_OtherProductDetails_MerchType");

            entity.HasOne(d => d.Product).WithOne(p => p.OtherProductDetail)
                .HasForeignKey<OtherProductDetail>(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OtherProductDetails_ProductInfo");

            entity.HasOne(d => d.Supplier).WithMany(p => p.OtherProductDetails)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OtherProductDetails_Supplier");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PaymentT__ED1FC9EA68A49FF1");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ConfirmedAt).HasColumnName("confirmed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Meta).HasColumnName("meta");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentCode)
                .HasDefaultValueSql("(NEXT VALUE FOR [dbo].[SeqPaymentCode])")
                .HasColumnName("payment_code");
            entity.Property(e => e.Provider)
                .HasMaxLength(50)
                .HasColumnName("provider");
            entity.Property(e => e.ProviderTxn)
                .HasMaxLength(100)
                .HasColumnName("provider_txn");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TxnType)
                .HasMaxLength(30)
                .HasColumnName("txn_type");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId).HasName("PK_Pet");

            entity.ToTable("Pet");

            entity.HasIndex(e => e.UserId, "Pet_index_23");

            entity.Property(e => e.BackgroundColor).HasMaxLength(50);
            entity.Property(e => e.PetId).HasColumnName("PetID");
            entity.Property(e => e.PetName).HasMaxLength(50);
            entity.Property(e => e.PointsChangedBackgroundColor).HasColumnName("PointsChanged_BackgroundColor");
            entity.Property(e => e.PointsChangedSkinColor).HasColumnName("PointsChanged_SkinColor");
            entity.Property(e => e.PointsGainedLevelUp).HasColumnName("PointsGained_LevelUp");
            entity.Property(e => e.PointsGainedTimeLevelUp).HasColumnName("PointsGainedTime_LevelUp");
            entity.Property(e => e.SkinColor)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pet__UserID__6E8B6712");
        });

        modelBuilder.Entity<PlayerMarketOrderInfo>(entity =>
        {
            entity.HasKey(e => e.POrderId).HasName("PK__PlayerMa__AACAAD70C933BAFC");

            entity.ToTable("PlayerMarketOrderInfo");

            entity.Property(e => e.POrderId)
                .ValueGeneratedNever()
                .HasColumnName("p_order_id");
            entity.Property(e => e.BuyerId).HasColumnName("buyer_id");
            entity.Property(e => e.POrderCreatedAt).HasColumnName("p_order_created_at");
            entity.Property(e => e.POrderDate).HasColumnName("p_order_date");
            entity.Property(e => e.POrderStatus)
                .HasMaxLength(1)
                .HasColumnName("p_order_status");
            entity.Property(e => e.POrderTotal).HasColumnName("p_order_total");
            entity.Property(e => e.POrderUpdatedAt).HasColumnName("p_order_updated_at");
            entity.Property(e => e.PPaymentStatus)
                .HasMaxLength(1)
                .HasColumnName("p_payment_status");
            entity.Property(e => e.PProductId).HasColumnName("p_product_id");
            entity.Property(e => e.PQuantity).HasColumnName("p_quantity");
            entity.Property(e => e.PUnitPrice).HasColumnName("p_unit_price");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");

            entity.HasOne(d => d.Buyer).WithMany(p => p.PlayerMarketOrderInfoBuyers)
                .HasForeignKey(d => d.BuyerId)
                .HasConstraintName("FK__PlayerMar__buyer__0A338187");

            entity.HasOne(d => d.PProduct).WithMany(p => p.PlayerMarketOrderInfos)
                .HasForeignKey(d => d.PProductId)
                .HasConstraintName("FK__PlayerMar__p_pro__084B3915");

            entity.HasOne(d => d.Seller).WithMany(p => p.PlayerMarketOrderInfoSellers)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK__PlayerMar__selle__093F5D4E");
        });

        modelBuilder.Entity<PlayerMarketOrderTradepage>(entity =>
        {
            entity.HasKey(e => e.POrderTradepageId).HasName("PK__PlayerMa__4E2C726D362DA57F");

            entity.ToTable("PlayerMarketOrderTradepage");

            entity.Property(e => e.POrderTradepageId)
                .ValueGeneratedNever()
                .HasColumnName("p_order_tradepage_id");
            entity.Property(e => e.BuyerReceivedAt).HasColumnName("buyer_received_at");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.POrderId).HasColumnName("p_order_id");
            entity.Property(e => e.POrderPlatformFee).HasColumnName("p_order_platform_fee");
            entity.Property(e => e.PProductId).HasColumnName("p_product_id");
            entity.Property(e => e.SellerTransferredAt).HasColumnName("seller_transferred_at");

            entity.HasOne(d => d.POrder).WithMany(p => p.PlayerMarketOrderTradepages)
                .HasForeignKey(d => d.POrderId)
                .HasConstraintName("FK__PlayerMar__p_ord__0B27A5C0");

            entity.HasOne(d => d.PProduct).WithMany(p => p.PlayerMarketOrderTradepages)
                .HasForeignKey(d => d.PProductId)
                .HasConstraintName("FK__PlayerMar__p_pro__0C1BC9F9");
        });

        modelBuilder.Entity<PlayerMarketProductImg>(entity =>
        {
            entity.HasKey(e => e.PProductImgId).HasName("PK__PlayerMa__75AAE6F0C6CBADFB");

            entity.Property(e => e.PProductImgId)
                .ValueGeneratedNever()
                .HasColumnName("p_product_img_id");
            entity.Property(e => e.PProductId).HasColumnName("p_product_id");
            entity.Property(e => e.PProductImgUrl).HasColumnName("p_product_img_url");

            entity.HasOne(d => d.PProduct).WithMany(p => p.PlayerMarketProductImgs)
                .HasForeignKey(d => d.PProductId)
                .HasConstraintName("FK__PlayerMar__p_pro__075714DC");
        });

        modelBuilder.Entity<PlayerMarketProductInfo>(entity =>
        {
            entity.HasKey(e => e.PProductId).HasName("PK__PlayerMa__A33C81652AD0DD1E");

            entity.ToTable("PlayerMarketProductInfo");

            entity.Property(e => e.PProductId)
                .ValueGeneratedNever()
                .HasColumnName("p_product_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.PProductDescription)
                .HasMaxLength(1)
                .HasColumnName("p_product_description");
            entity.Property(e => e.PProductImgId)
                .HasMaxLength(1)
                .HasColumnName("p_product_img_id");
            entity.Property(e => e.PProductName)
                .HasMaxLength(1)
                .HasColumnName("p_product_name");
            entity.Property(e => e.PProductTitle)
                .HasMaxLength(1)
                .HasColumnName("p_product_title");
            entity.Property(e => e.PProductType)
                .HasMaxLength(1)
                .HasColumnName("p_product_type");
            entity.Property(e => e.PStatus)
                .HasMaxLength(1)
                .HasColumnName("p_status");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Seller).WithMany(p => p.PlayerMarketProductInfos)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK__PlayerMar__selle__0662F0A3");
        });

        modelBuilder.Entity<PlayerMarketRanking>(entity =>
        {
            entity.HasKey(e => e.PRankingId).HasName("PK__PlayerMa__2B50ED32026B4213");

            entity.ToTable("PlayerMarketRanking");

            entity.Property(e => e.PRankingId)
                .ValueGeneratedNever()
                .HasColumnName("p_ranking_id");
            entity.Property(e => e.PPeriodType)
                .HasMaxLength(255)
                .HasColumnName("p_period_type");
            entity.Property(e => e.PProductId).HasColumnName("p_product_id");
            entity.Property(e => e.PRankingDate).HasColumnName("p_ranking_date");
            entity.Property(e => e.PRankingMetric)
                .HasMaxLength(255)
                .HasColumnName("p_ranking_metric");
            entity.Property(e => e.PRankingPosition).HasColumnName("p_ranking_position");
            entity.Property(e => e.PTradingAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("p_trading_amount");
            entity.Property(e => e.PTradingVolume).HasColumnName("p_trading_volume");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.PProduct).WithMany(p => p.PlayerMarketRankings)
                .HasForeignKey(d => d.PProductId)
                .HasConstraintName("FK__PlayerMar__p_pro__047AA831");
        });

        modelBuilder.Entity<PlayerMarketTradeMsg>(entity =>
        {
            entity.HasKey(e => e.TradeMsgId).HasName("PK__PlayerMa__C2FA77A23EC155AE");

            entity.ToTable("PlayerMarketTradeMsg");

            entity.Property(e => e.TradeMsgId)
                .ValueGeneratedNever()
                .HasColumnName("trade_msg_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.MessageText)
                .HasMaxLength(1)
                .HasColumnName("message_text");
            entity.Property(e => e.MsgFrom)
                .HasMaxLength(1)
                .HasColumnName("msg_from");
            entity.Property(e => e.POrderTradepageId).HasColumnName("p_order_tradepage_id");

            entity.HasOne(d => d.POrderTradepage).WithMany(p => p.PlayerMarketTradeMsgs)
                .HasForeignKey(d => d.POrderTradepageId)
                .HasConstraintName("FK__PlayerMar__p_ord__0D0FEE32");
        });

        modelBuilder.Entity<PopularityIndexDaily>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__populari__3213E83F08C19225");

            entity.ToTable("popularity_index_daily");

            entity.HasIndex(e => new { e.GameId, e.Date }, "popularity_index_daily_index_6").IsUnique();

            entity.HasIndex(e => e.Date, "popularity_index_daily_index_7");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.IndexValue)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("index_value");

            entity.HasOne(d => d.Game).WithMany(p => p.PopularityIndexDailies)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__popularit__game___0539C240");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__posts__3ED78766F2C1BEAE");

            entity.ToTable("posts");

            entity.HasIndex(e => new { e.Type, e.CreatedAt }, "posts_index_11");

            entity.HasIndex(e => new { e.GameId, e.CreatedAt }, "posts_index_12");

            entity.HasIndex(e => new { e.Status, e.CreatedAt }, "posts_index_13");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.BodyMd)
                .HasMaxLength(3000)
                .HasColumnName("body_md");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.Pinned).HasColumnName("pinned");
            entity.Property(e => e.PublishedAt).HasColumnName("published_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.Tldr)
                .HasMaxLength(50)
                .HasColumnName("tldr");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__posts__created_b__0169315C");

            entity.HasOne(d => d.Game).WithMany(p => p.Posts)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__posts__game_id__07220AB2");
        });

        modelBuilder.Entity<PostMetricSnapshot>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__post_met__3ED7876644BB3452");

            entity.ToTable("post_metric_snapshot");

            entity.Property(e => e.PostId)
                .ValueGeneratedNever()
                .HasColumnName("post_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.DetailsJson).HasColumnName("details_json");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.IndexValue)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("index_value");

            entity.HasOne(d => d.Game).WithMany(p => p.PostMetricSnapshots)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__post_metr__game___035179CE");

            entity.HasOne(d => d.Post).WithOne(p => p.PostMetricSnapshot)
                .HasForeignKey<PostMetricSnapshot>(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__post_metr__post___025D5595");
        });

        modelBuilder.Entity<PostSource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__post_sou__3213E83FA633BB17");

            entity.ToTable("post_sources");

            entity.HasIndex(e => e.PostId, "post_sources_index_14");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.SourceName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("source_name");
            entity.Property(e => e.Url)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("url");

            entity.HasOne(d => d.Post).WithMany(p => p.PostSources)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__post_sour__post___04459E07");
        });

        modelBuilder.Entity<ProductCode>(entity =>
        {
            entity.HasKey(e => e.ProductCode1).HasName("PK__ProductC__AE1A8CC502EA2584");

            entity.ToTable("ProductCode");

            entity.HasIndex(e => e.ProductId, "UQ_ProductCode_ProductId").IsUnique();

            entity.Property(e => e.ProductCode1)
                .HasMaxLength(50)
                .HasColumnName("product_code");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithOne(p => p.ProductCode)
                .HasForeignKey<ProductCode>(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCode_ProductInfo");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ProductimgId).HasName("PK__ProductI__4FFACE1528BCC4AC");

            entity.Property(e => e.ProductimgId).HasColumnName("productimg_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductimgAltText)
                .HasMaxLength(255)
                .HasColumnName("productimg_alt_text");
            entity.Property(e => e.ProductimgUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("productimg_updated_at");
            entity.Property(e => e.ProductimgUrl)
                .HasMaxLength(500)
                .HasColumnName("productimg_url");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_ProductInfo");
        });

        modelBuilder.Entity<ProductInfo>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__ProductI__47027DF58E47EAE3");

            entity.ToTable("ProductInfo");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(10)
                .HasDefaultValue("TWD")
                .HasColumnName("currency_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("product_created_at");
            entity.Property(e => e.ProductCreatedBy).HasColumnName("product_created_by");
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductType)
                .HasMaxLength(200)
                .HasColumnName("product_type");
            entity.Property(e => e.ProductUpdatedAt).HasColumnName("product_updated_at");
            entity.Property(e => e.ProductUpdatedBy).HasColumnName("product_updated_by");
            entity.Property(e => e.ShipmentQuantity).HasColumnName("Shipment_Quantity");

            entity.HasOne(d => d.ProductCreatedByNavigation).WithMany(p => p.ProductInfoProductCreatedByNavigations)
                .HasForeignKey(d => d.ProductCreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductInfo_ManagerData");

            entity.HasOne(d => d.ProductUpdatedByNavigation).WithMany(p => p.ProductInfoProductUpdatedByNavigations)
                .HasForeignKey(d => d.ProductUpdatedBy)
                .HasConstraintName("FK_ProductInfo_ManagerData_updatedby");
        });

        modelBuilder.Entity<ProductInfoAuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__ProductI__9E2397E0E26EBC39");

            entity.ToTable("ProductInfoAuditLog");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(30)
                .HasColumnName("action_type");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("changed_at");
            entity.Property(e => e.FieldName)
                .HasMaxLength(100)
                .HasColumnName("field_name");
            entity.Property(e => e.ManagerId).HasColumnName("Manager_Id");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.ProductInfoAuditLogs)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__ProductInfoAuditLog_ManagerData");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductInfoAuditLogs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductInfoAuditLog_ProductInfo");
        });

        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__reaction__3213E83F940B04A4");

            entity.ToTable("reactions");

            entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId, e.Kind }, "reactions_index_18").IsUnique();

            entity.HasIndex(e => new { e.TargetType, e.TargetId }, "reactions_index_19");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Kind)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("kind");
            entity.Property(e => e.TargetId).HasColumnName("target_id");
            entity.Property(e => e.TargetType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("target_type");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Reactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__reactions__User___6ABAD62E");
        });

        modelBuilder.Entity<Relation>(entity =>
        {
            entity.ToTable("Relation");

            entity.HasIndex(e => new { e.UserIdSmall, e.UserIdLarge }, "UQ_Relation_UserPair").IsUnique();

            entity.Property(e => e.RelationId).HasColumnName("relation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.FriendNickname)
                .HasMaxLength(10)
                .HasColumnName("friend_nickname");
            entity.Property(e => e.RequestedBy).HasColumnName("requested_by");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserIdLarge).HasColumnName("user_id_large");
            entity.Property(e => e.UserIdSmall).HasColumnName("user_id_small");

            entity.HasOne(d => d.RequestedByNavigation).WithMany(p => p.RelationRequestedByNavigations)
                .HasForeignKey(d => d.RequestedBy)
                .HasConstraintName("FK_Relation_RequestedBy");

            entity.HasOne(d => d.Status).WithMany(p => p.Relations)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Relation_Status");

            entity.HasOne(d => d.UserIdLargeNavigation).WithMany(p => p.RelationUserIdLargeNavigations)
                .HasForeignKey(d => d.UserIdLarge)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Relation_UserLarge");

            entity.HasOne(d => d.UserIdSmallNavigation).WithMany(p => p.RelationUserIdSmallNavigations)
                .HasForeignKey(d => d.UserIdSmall)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Relation_UserSmall");
        });

        modelBuilder.Entity<RelationStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("Relation_Status");

            entity.HasIndex(e => e.StatusCode, "UQ_Relation_Status_Code").IsUnique();

            entity.HasIndex(e => e.StatusName, "UQ_Relation_Status_Name").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(30)
                .HasColumnName("status_code");
            entity.Property(e => e.StatusName)
                .HasMaxLength(30)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.ShipmentId).HasName("PK__Shipment__41466E5960054AD2");

            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.Carrier)
                .HasMaxLength(50)
                .HasColumnName("carrier");
            entity.Property(e => e.DeliveredAt).HasColumnName("delivered_at");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ShipmentCode)
                .HasDefaultValueSql("(NEXT VALUE FOR [dbo].[SeqShipmentCode])")
                .HasColumnName("shipment_code");
            entity.Property(e => e.ShippedAt).HasColumnName("shipped_at");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TrackingNo)
                .HasMaxLength(100)
                .HasColumnName("tracking_no");
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.MovementId).HasName("PK__StockMov__AB1D1022C599FC06");

            entity.Property(e => e.MovementId).HasColumnName("movement_id");
            entity.Property(e => e.ChangeQty).HasColumnName("change_qty");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(30)
                .HasColumnName("reason");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__6EE594E853946201");

            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .HasColumnName("supplier_name");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId);

            entity.ToTable("Support_Tickets");

            entity.HasIndex(e => new { e.IsClosed, e.AssignedManagerId, e.LastMessageAt }, "IX_Support_Tickets_Closed_Assigned_LastMsg").IsDescending(false, false, true);

            entity.HasIndex(e => new { e.UserId, e.LastMessageAt }, "IX_Support_Tickets_User_LastMsg").IsDescending(false, true);

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.AssignedManagerId).HasColumnName("assigned_manager_id");
            entity.Property(e => e.CloseNote)
                .HasMaxLength(255)
                .HasColumnName("close_note");
            entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
            entity.Property(e => e.ClosedByManagerId).HasColumnName("closed_by_manager_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsClosed).HasColumnName("is_closed");
            entity.Property(e => e.LastMessageAt).HasColumnName("last_message_at");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AssignedManager).WithMany(p => p.SupportTicketAssignedManagers)
                .HasForeignKey(d => d.AssignedManagerId)
                .HasConstraintName("FK_Support_Tickets_AssignedManager");

            entity.HasOne(d => d.ClosedByManager).WithMany(p => p.SupportTicketClosedByManagers)
                .HasForeignKey(d => d.ClosedByManagerId)
                .HasConstraintName("FK_Support_Tickets_ClosedBy");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Support_Tickets_Users");
        });

        modelBuilder.Entity<SupportTicketAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.ToTable("Support_Ticket_Assignments");

            entity.HasIndex(e => new { e.TicketId, e.AssignedAt }, "IX_Ticket_Assignments_Ticket_Time");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedByManagerId).HasColumnName("assigned_by_manager_id");
            entity.Property(e => e.FromManagerId).HasColumnName("from_manager_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.ToManagerId).HasColumnName("to_manager_id");

            entity.HasOne(d => d.AssignedByManager).WithMany(p => p.SupportTicketAssignmentAssignedByManagers)
                .HasForeignKey(d => d.AssignedByManagerId)
                .HasConstraintName("FK_Ticket_Assignments_By");

            entity.HasOne(d => d.FromManager).WithMany(p => p.SupportTicketAssignmentFromManagers)
                .HasForeignKey(d => d.FromManagerId)
                .HasConstraintName("FK_Ticket_Assignments_From");

            entity.HasOne(d => d.Ticket).WithMany(p => p.SupportTicketAssignments)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_Ticket_Assignments_Ticket");

            entity.HasOne(d => d.ToManager).WithMany(p => p.SupportTicketAssignmentToManagers)
                .HasForeignKey(d => d.ToManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Assignments_To");
        });

        modelBuilder.Entity<SupportTicketMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("Support_Ticket_Messages");

            entity.HasIndex(e => new { e.TicketId, e.SentAt }, "IX_Support_Ticket_Messages_Ticket_SentAt");

            entity.HasIndex(e => new { e.TicketId, e.SentAt }, "IX_Support_Ticket_Messages_Unread_ForManager").HasFilter("([read_by_manager_at] IS NULL)");

            entity.HasIndex(e => new { e.TicketId, e.SentAt }, "IX_Support_Ticket_Messages_Unread_ForUser").HasFilter("([read_by_user_at] IS NULL)");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.MessageText)
                .HasMaxLength(255)
                .HasColumnName("message_text");
            entity.Property(e => e.ReadByManagerAt).HasColumnName("read_by_manager_at");
            entity.Property(e => e.ReadByUserAt).HasColumnName("read_by_user_at");
            entity.Property(e => e.SenderManagerId).HasColumnName("sender_manager_id");
            entity.Property(e => e.SenderUserId).HasColumnName("sender_user_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("sent_at");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.SenderManager).WithMany(p => p.SupportTicketMessages)
                .HasForeignKey(d => d.SenderManagerId)
                .HasConstraintName("FK_Support_Ticket_Messages_SenderManager");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.SupportTicketMessages)
                .HasForeignKey(d => d.SenderUserId)
                .HasConstraintName("FK_Support_Ticket_Messages_SenderUser");

            entity.HasOne(d => d.Ticket).WithMany(p => p.SupportTicketMessages)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_Support_Ticket_Messages_Tickets");
        });

        modelBuilder.Entity<Thread>(entity =>
        {
            entity.HasKey(e => e.ThreadId).HasName("PK__threads__7411E2F035E8CC2A");

            entity.ToTable("threads");

            entity.HasIndex(e => new { e.ForumId, e.UpdatedAt }, "threads_index_16");

            entity.Property(e => e.ThreadId).HasColumnName("thread_id");
            entity.Property(e => e.AuthorUserId).HasColumnName("author_User_ID");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ForumId).HasColumnName("forum_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.AuthorUser).WithMany(p => p.Threads)
                .HasForeignKey(d => d.AuthorUserId)
                .HasConstraintName("FK__threads__author___66EA454A");

            entity.HasOne(d => d.Forum).WithMany(p => p.Threads)
                .HasForeignKey(d => d.ForumId)
                .HasConstraintName("FK__threads__forum_i__00750D23");
        });

        modelBuilder.Entity<ThreadPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__thread_p__3213E83F966AF56F");

            entity.ToTable("thread_posts");

            entity.HasIndex(e => new { e.ThreadId, e.CreatedAt }, "thread_posts_index_17");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorUserId).HasColumnName("author_User_ID");
            entity.Property(e => e.ContentMd)
                .HasMaxLength(3000)
                .HasColumnName("content_md");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ParentPostId).HasColumnName("parent_post_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.ThreadId).HasColumnName("thread_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.AuthorUser).WithMany(p => p.ThreadPosts)
                .HasForeignKey(d => d.AuthorUserId)
                .HasConstraintName("FK__thread_po__autho__68D28DBC");

            entity.HasOne(d => d.ParentPost).WithMany(p => p.InverseParentPost)
                .HasForeignKey(d => d.ParentPostId)
                .HasConstraintName("FK__thread_po__paren__69C6B1F5");

            entity.HasOne(d => d.Thread).WithMany(p => p.ThreadPosts)
                .HasForeignKey(d => d.ThreadId)
                .HasConstraintName("FK__thread_po__threa__67DE6983");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__206D9190FA40893F");

            entity.HasIndex(e => e.UserName, "UQ__Users__5F1A108682A83552").IsUnique();

            entity.HasIndex(e => e.UserAccount, "UQ__Users__899F4A91E5EF8DB8").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.UserAccessFailedCount).HasColumnName("User_AccessFailedCount");
            entity.Property(e => e.UserAccount)
                .HasMaxLength(30)
                .HasColumnName("User_Account");
            entity.Property(e => e.UserEmailConfirmed).HasColumnName("User_EmailConfirmed");
            entity.Property(e => e.UserLockoutEnabled)
                .HasDefaultValue(true)
                .HasColumnName("User_LockoutEnabled");
            entity.Property(e => e.UserLockoutEnd).HasColumnName("User_LockoutEnd");
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .HasColumnName("User_name");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(30)
                .HasColumnName("User_Password");
            entity.Property(e => e.UserPhoneNumberConfirmed).HasColumnName("User_PhoneNumberConfirmed");
            entity.Property(e => e.UserTwoFactorEnabled).HasColumnName("User_TwoFactorEnabled");
        });

        modelBuilder.Entity<UserIntroduce>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User_Int__206D91909E25154A");

            entity.ToTable("User_Introduce");

            entity.HasIndex(e => e.IdNumber, "UQ__User_Int__62DF8033EDDD19FD").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User_Int__A9D10534CAA16AC5").IsUnique();

            entity.HasIndex(e => e.Cellphone, "UQ__User_Int__CDE19CF29F238F26").IsUnique();

            entity.HasIndex(e => e.UserNickName, "UQ__User_Int__DAFD02CF797EB192").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("User_ID");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Cellphone)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.CreateAccount).HasColumnName("Create_Account");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IdNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UserIntroduce1)
                .HasMaxLength(200)
                .HasColumnName("User_Introduce");
            entity.Property(e => e.UserNickName)
                .HasMaxLength(50)
                .HasColumnName("User_NickName");
            entity.Property(e => e.UserPicture).HasColumnName("User_Picture");

            entity.HasOne(d => d.User).WithOne(p => p.UserIntroduce)
                .HasForeignKey<UserIntroduce>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Intr__User___0FB750B3");
        });

        modelBuilder.Entity<UserRight>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User_Rig__206D91702DB5DE3E");

            entity.ToTable("User_Rights");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("User_Id");
            entity.Property(e => e.UserStatus).HasColumnName("User_Status");

            entity.HasOne(d => d.User).WithOne(p => p.UserRight)
                .HasForeignKey<UserRight>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Righ__User___10AB74EC");
        });

        modelBuilder.Entity<UserSalesInformation>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User_Sal__206D9170797CCA09");

            entity.ToTable("User_Sales_Information");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("User_Id");
            entity.Property(e => e.UserSalesWallet).HasColumnName("UserSales_Wallet");

            entity.HasOne(d => d.User).WithOne(p => p.UserSalesInformation)
                .HasForeignKey<UserSalesInformation>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Sale__User___1293BD5E");
        });

        modelBuilder.Entity<UserSignInStat>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK_UserSignInStats");

            entity.HasIndex(e => new { e.UserId, e.SignTime }, "UserSignInStats_index_22");

            entity.Property(e => e.CouponGained)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSignI__UserI__6D9742D9");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__UserToke__AA16D540EFFF863A");

            entity.Property(e => e.TokenId).HasColumnName("Token_ID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.Value).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserToken__User___0EC32C7A");
        });

        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User_Wallet");

            entity.ToTable("User_Wallet");

            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.UserPoint).HasColumnName("User_Point");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Wall__User___6CA31EA0");
        });

        modelBuilder.Entity<VCsEligibleAgent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCS_EligibleAgents");
        });

        modelBuilder.Entity<WalletHistory>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK_WalletHistory");

            entity.ToTable("WalletHistory");

            entity.HasIndex(e => new { e.UserId, e.ChangeTime }, "WalletHistory_index_30");

            entity.HasIndex(e => new { e.ChangeType, e.ChangeTime }, "WalletHistory_index_31");

            entity.Property(e => e.ChangeType).HasMaxLength(10);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ItemCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WalletHis__UserI__62E4AA3C");
        });
        modelBuilder.HasSequence("SeqOrderCode")
            .StartsAt(100000000001L)
            .HasMin(100000000001L)
            .HasMax(999999999999L);
        modelBuilder.HasSequence("SeqPaymentCode")
            .StartsAt(1000000000000001L)
            .HasMin(1000000000000001L)
            .HasMax(9999999999999999L);
        modelBuilder.HasSequence("SeqShipmentCode")
            .StartsAt(10000000000001L)
            .HasMin(10000000000001L)
            .HasMax(99999999999999L);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
