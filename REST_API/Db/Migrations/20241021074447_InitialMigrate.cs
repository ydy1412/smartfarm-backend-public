using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace REST_API.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    TypeCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ParentMetaDataId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadata_metadata_ParentMetaDataId",
                        column: x => x.ParentMetaDataId,
                        principalTable: "metadata",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DepositAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsFarmManager = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metadata-hierarchy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    ChildId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metadata-hierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_metadata-hierarchy_metadata_ChildId",
                        column: x => x.ChildId,
                        principalTable: "metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metadata-hierarchy_metadata_ParentId",
                        column: x => x.ParentId,
                        principalTable: "metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "farm_manager",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm_manager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_farm_manager_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "farm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FarmManagerId = table.Column<int>(type: "integer", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_farm_farm_manager_FarmManagerId",
                        column: x => x.FarmManagerId,
                        principalTable: "farm_manager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FarmId = table.Column<int>(type: "integer", nullable: false),
                    FacilityTypeId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_facility_farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_facility_metadata_FacilityTypeId",
                        column: x => x.FacilityTypeId,
                        principalTable: "metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "farm_crop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FarmId = table.Column<int>(type: "integer", nullable: false),
                    CropTypeId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm_crop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_farm_crop_farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_farm_crop_metadata_CropTypeId",
                        column: x => x.CropTypeId,
                        principalTable: "metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "farm_unit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FarmId = table.Column<int>(type: "integer", nullable: false),
                    FarmUnitTypeId = table.Column<int>(type: "integer", nullable: false),
                    FarmUnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm_unit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_farm_unit_farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_farm_unit_metadata_FarmUnitTypeId",
                        column: x => x.FarmUnitTypeId,
                        principalTable: "metadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_farm_unit_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "farm_sale_offer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FarmUnitId = table.Column<int>(type: "integer", nullable: false),
                    SuggestedPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TransactionStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm_sale_offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_farm_sale_offer_farm_unit_FarmUnitId",
                        column: x => x.FarmUnitId,
                        principalTable: "farm_unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "farm_sale_order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FarmSaleOfferId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farm_sale_order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_farm_sale_order_farm_sale_offer_FarmSaleOfferId",
                        column: x => x.FarmSaleOfferId,
                        principalTable: "farm_sale_offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_farm_sale_order_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_facility_FacilityTypeId",
                table: "facility",
                column: "FacilityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_facility_FarmId",
                table: "facility",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_FarmManagerId",
                table: "farm",
                column: "FarmManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_crop_CropTypeId",
                table: "farm_crop",
                column: "CropTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_crop_FarmId",
                table: "farm_crop",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_manager_UserId",
                table: "farm_manager",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_farm_sale_offer_FarmUnitId",
                table: "farm_sale_offer",
                column: "FarmUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_sale_order_FarmSaleOfferId",
                table: "farm_sale_order",
                column: "FarmSaleOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_sale_order_UserId",
                table: "farm_sale_order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_unit_FarmId",
                table: "farm_unit",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_unit_FarmUnitTypeId",
                table: "farm_unit",
                column: "FarmUnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_farm_unit_UserId",
                table: "farm_unit",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_metadata_ParentMetaDataId",
                table: "metadata",
                column: "ParentMetaDataId");

            migrationBuilder.CreateIndex(
                name: "IX_metadata-hierarchy_ChildId",
                table: "metadata-hierarchy",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_metadata-hierarchy_ParentId",
                table: "metadata-hierarchy",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "facility");

            migrationBuilder.DropTable(
                name: "farm_crop");

            migrationBuilder.DropTable(
                name: "farm_sale_order");

            migrationBuilder.DropTable(
                name: "metadata-hierarchy");

            migrationBuilder.DropTable(
                name: "farm_sale_offer");

            migrationBuilder.DropTable(
                name: "farm_unit");

            migrationBuilder.DropTable(
                name: "farm");

            migrationBuilder.DropTable(
                name: "metadata");

            migrationBuilder.DropTable(
                name: "farm_manager");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
