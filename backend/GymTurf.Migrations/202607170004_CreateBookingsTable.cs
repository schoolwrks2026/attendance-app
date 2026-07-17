using FluentMigrator;

namespace GymTurf.Migrations;

[Migration(202607170004)]
public class CreateBookingsTable : Migration
{
    public override void Up()
    {
        Create.Table("Bookings")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("TenantId").AsGuid().NotNullable()
            .WithColumn("FacilityId").AsGuid().NotNullable().ForeignKey("FK_Bookings_Facilities", "Facilities", "Id")
            .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("FK_Bookings_Users", "Users", "Id")
            .WithColumn("MemberName").AsString(150).NotNullable()
            .WithColumn("BookingDate").AsDate().NotNullable()
            .WithColumn("StartTime").AsTime().NotNullable()
            .WithColumn("EndTime").AsTime().NotNullable()
            .WithColumn("Status").AsString(50).NotNullable().WithDefaultValue("Confirmed")
            .WithColumn("Notes").AsString(1000).Nullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().NotNullable()
            .WithColumn("CreatedBy").AsString(100).NotNullable()
            .WithColumn("UpdatedBy").AsString(100).NotNullable()
            .WithColumn("DeletedAt").AsDateTime().Nullable()
            .WithColumn("Version").AsInt32().NotNullable().WithDefaultValue(1);

        Create.Index("IX_Bookings_TenantId").OnTable("Bookings").OnColumn("TenantId");
        Create.Index("IX_Bookings_FacilityId").OnTable("Bookings").OnColumn("FacilityId");
        Create.Index("IX_Bookings_BookingDate").OnTable("Bookings").OnColumn("BookingDate");
        Create.Index("IX_Bookings_UserId").OnTable("Bookings").OnColumn("UserId");
    }

    public override void Down()
    {
        Delete.Table("Bookings");
    }
}
