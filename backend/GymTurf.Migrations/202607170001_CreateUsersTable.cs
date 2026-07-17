using FluentMigrator;

namespace GymTurf.Migrations;

[Migration(202607170001)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("TenantId").AsGuid().NotNullable()
            .WithColumn("Username").AsString(100).NotNullable().Unique()
            .WithColumn("Email").AsString(255).NotNullable().Unique()
            .WithColumn("PasswordHash").AsString(255).NotNullable()
            .WithColumn("Role").AsString(50).NotNullable()
            .WithColumn("FullName").AsString(100).NotNullable()
            .WithColumn("PhoneNumber").AsString(50).Nullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedBy").AsString(100).NotNullable()
            .WithColumn("UpdatedBy").AsString(100).NotNullable()
            .WithColumn("DeletedAt").AsDateTime().Nullable()
            .WithColumn("Version").AsInt32().NotNullable().WithDefaultValue(1);

        Create.Index("IX_Users_TenantId").OnTable("Users").OnColumn("TenantId");
    }

    public override void Down()
    {
        Delete.Table("Users");
    }
}
