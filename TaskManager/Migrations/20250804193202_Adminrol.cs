using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class Adminrol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT * FROM AspNetRoles WHERE Id = '01987694-60c4-7e0b-b1f3-ca230a57d1cf')
                BEGIN 
                    INSERT AspNetRoles ( Id, [Name], [NormalizedName])
                    VALUES ( '01987694-60c4-7e0b-b1f3-ca230a57d1cf', 'Admin', 'ADMIN')
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Id = '01987694-60c4-7e0b-b1f3-ca230a57d1cf'");
        }
    }
}
