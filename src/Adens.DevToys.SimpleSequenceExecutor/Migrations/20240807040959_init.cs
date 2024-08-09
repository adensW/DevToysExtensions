using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adens.DevToys.SimpleSequenceExecutor.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bundles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bundles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BundleSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    BundleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BundleSteps_Bundles_BundleId",
                        column: x => x.BundleId,
                        principalTable: "Bundles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BundleStepParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    StepId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleStepParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BundleStepParameters_BundleSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "BundleSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bundles_Name",
                table: "Bundles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleStepParameters_StepId",
                table: "BundleStepParameters",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_BundleSteps_BundleId",
                table: "BundleSteps",
                column: "BundleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BundleStepParameters");

            migrationBuilder.DropTable(
                name: "BundleSteps");

            migrationBuilder.DropTable(
                name: "Bundles");
        }
    }
}
