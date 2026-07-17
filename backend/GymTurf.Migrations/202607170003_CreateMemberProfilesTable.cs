using FluentMigrator;

namespace GymTurf.Migrations;

[Migration(202607170003)]
public class CreateMemberProfilesTable : Migration
{
    public override void Up()
    {
        Create.Table("MemberProfiles")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("TenantId").AsGuid().NotNullable()
            .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("FK_MemberProfiles_Users", "Users", "Id")
            .WithColumn("MembershipType").AsString(50).NotNullable() // None, Monthly, Annual, Premium
            .WithColumn("MembershipStartDate").AsDateTime().NotNullable()
            .WithColumn("MembershipEndDate").AsDateTime().NotNullable()
            .WithColumn("IsMembershipActive").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("MedicalNotes").AsString(1000).Nullable()
            .WithColumn("EmergencyContactName").AsString(100).Nullable()
            .WithColumn("EmergencyContactPhone").AsString(50).Nullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedBy").AsString(100).NotNullable()
            .WithColumn("UpdatedBy").AsString(100).NotNullable()
            .WithColumn("DeletedAt").AsDateTime().Nullable()
            .WithColumn("Version").AsInt32().NotNullable().WithDefaultValue(1);

        Create.Index("IX_MemberProfiles_TenantId").OnTable("MemberProfiles").OnColumn("TenantId");
        Create.Index("IX_MemberProfiles_UserId").OnTable("MemberProfiles").OnColumn("UserId");
    }

    public override void Down()
    {
        Delete.Table("MemberProfiles");
    }
}
