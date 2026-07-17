using FluentMigrator;

namespace GymTurf.Migrations;

[Migration(202607170002)]
public class CreateBranchesAndFacilitiesTables : Migration
{
    public override void Up()
    {
        Create.Table("Branches")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("TenantId").AsGuid().NotNullable()
            .WithColumn("Name").AsString(150).NotNullable()
            .WithColumn("Address").AsString(255).NotNullable()
            .WithColumn("PhoneNumber").AsString(50).Nullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedBy").AsString(100).NotNullable()
            .WithColumn("UpdatedBy").AsString(100).NotNullable()
            .WithColumn("DeletedAt").AsDateTime().Nullable()
            .WithColumn("Version").AsInt32().NotNullable().WithDefaultValue(1);

        Create.Table("Facilities")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("TenantId").AsGuid().NotNullable()
            .WithColumn("BranchId").AsGuid().NotNullable().ForeignKey("FK_Facilities_Branches", "Branches", "Id")
            .WithColumn("Name").AsString(150).NotNullable()
            .WithColumn("Type").AsString(50).NotNullable() // Gym, Turf, Court
            .WithColumn("Capacity").AsInt32().NotNullable()
            .WithColumn("PricePerHour").AsDecimal(18, 2).NotNullable().WithDefaultValue(0.00)
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedBy").AsString(100).NotNullable()
            .WithColumn("UpdatedBy").AsString(100).NotNullable()
            .WithColumn("DeletedAt").AsDateTime().Nullable()
            .WithColumn("Version").AsInt32().NotNullable().WithDefaultValue(1);

        Create.Index("IX_Branches_TenantId").OnTable("Branches").OnColumn("TenantId");
        Create.Index("IX_Facilities_TenantId").OnTable("Facilities").OnColumn("TenantId");
        Create.Index("IX_Facilities_BranchId").OnTable("Facilities").OnColumn("BranchId");
    }

    public override void Down()
    {
        Delete.Table("Facilities");
        Delete.Table("Branches");
    }
}
