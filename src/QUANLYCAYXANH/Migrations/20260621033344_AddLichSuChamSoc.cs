using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QUANLYCAYXANH.Migrations
{
    /// <inheritdoc />
    public partial class AddLichSuChamSoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhuVuc",
                columns: table => new
                {
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhuVuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuVuc", x => x.MaKhuVuc);
                });

            migrationBuilder.CreateTable(
                name: "CayXanh",
                columns: table => new
                {
                    MaCay = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiCay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTrong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiPhuTrach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CayXanh", x => x.MaCay);
                    table.ForeignKey(
                        name: "FK_CayXanh_KhuVuc_MaKhuVuc",
                        column: x => x.MaKhuVuc,
                        principalTable: "KhuVuc",
                        principalColumn: "MaKhuVuc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuChamSoc",
                columns: table => new
                {
                    MaChamSoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCay = table.Column<int>(type: "int", nullable: false),
                    LoaiChamSoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayChamSoc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CayXanhMaCay = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuChamSoc", x => x.MaChamSoc);
                    table.ForeignKey(
                        name: "FK_LichSuChamSoc_CayXanh_CayXanhMaCay",
                        column: x => x.CayXanhMaCay,
                        principalTable: "CayXanh",
                        principalColumn: "MaCay");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CayXanh_MaKhuVuc",
                table: "CayXanh",
                column: "MaKhuVuc");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChamSoc_CayXanhMaCay",
                table: "LichSuChamSoc",
                column: "CayXanhMaCay");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuChamSoc");

            migrationBuilder.DropTable(
                name: "CayXanh");

            migrationBuilder.DropTable(
                name: "KhuVuc");
        }
    }
}
