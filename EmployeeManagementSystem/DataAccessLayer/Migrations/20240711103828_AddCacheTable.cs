using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddCacheTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE dbo.CacheTable (
                    Id nvarchar(449) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
                    Value varbinary(MAX) NOT NULL,
                    ExpiresAtTime datetimeoffset NOT NULL,
                    SlidingExpirationInSeconds bigint NULL,
                    AbsoluteExpiration datetimeoffset NULL,
                    PRIMARY KEY (Id)
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE dbo.CacheTable;");
        }
    }
}
