using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentLifeApp_BE.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleID", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Teacher" },
                    { 3, "Student" }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "SubjectID", "Credits", "Description", "SubjectName" },
                values: new object[,]
                {
                    { 1, 3, "Basic mathematics and algebra", "Mathematics" },
                    { 2, 4, "Introductory physics and mechanics", "Physics" },
                    { 3, 2, "English communication and writing", "English" },
                    { 4, 3, "General chemistry and reactions", "Chemistry" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "CreatedAt", "Email", "FullName", "PasswordHash", "RoleID", "StudentCode" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), "admin@lms.local", "Admin User", "admin-placeholder-hash", 1, null },
                    { 2, new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), "teacher1@lms.local", "Teacher One", "teacher-placeholder-hash", 2, null },
                    { 3, new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), "student1@lms.local", "Student One", "student-placeholder-hash", 3, "S001" },
                    { 4, new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), "student2@lms.local", "Student Two", "student2-placeholder-hash", 3, "S002" },
                    { 5, new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), "student3@lms.local", "Student Three", "student3-placeholder-hash", 3, "S003" },
                    { 6, new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), "student4@lms.local", "Student Four", "student4-placeholder-hash", 3, "S004" }
                });

            migrationBuilder.InsertData(
                table: "Assignments",
                columns: new[] { "AssignmentID", "CreatedBy", "Deadline", "MaxScore", "SubjectID", "Title", "Weight" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 3, 11, 23, 59, 0, 0, DateTimeKind.Utc), 10.0, 1, "Math Homework 1", 0.20000000000000001 },
                    { 2, 2, new DateTime(2026, 3, 18, 23, 59, 0, 0, DateTimeKind.Utc), 10.0, 1, "Math Homework 2", 0.20000000000000001 },
                    { 3, 2, new DateTime(2026, 3, 14, 23, 59, 0, 0, DateTimeKind.Utc), 15.0, 2, "Physics Lab Report", 0.29999999999999999 },
                    { 4, 2, new DateTime(2026, 3, 21, 23, 59, 0, 0, DateTimeKind.Utc), 15.0, 2, "Physics Experiment", 0.29999999999999999 },
                    { 5, 2, new DateTime(2026, 3, 9, 23, 59, 0, 0, DateTimeKind.Utc), 20.0, 3, "English Essay", 0.5 },
                    { 6, 2, new DateTime(2026, 3, 17, 23, 59, 0, 0, DateTimeKind.Utc), 12.0, 4, "Chemistry Lab", 0.25 }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "EnrollmentID", "IsTeacher", "SubjectID", "UserID" },
                values: new object[,]
                {
                    { 1, true, 1, 2 },
                    { 2, true, 2, 2 },
                    { 3, true, 3, 2 },
                    { 4, true, 4, 2 },
                    { 5, false, 1, 3 },
                    { 6, false, 2, 3 },
                    { 7, false, 1, 4 },
                    { 8, false, 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "NewsID", "Content", "CreatedAt", "CreatedBy", "IsActive", "TargetRoleID", "Title" },
                values: new object[,]
                {
                    { 1, "This is a seeded announcement for all users. Welcome to our Learning Management System!", new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), 1, true, null, "Welcome to LMS" },
                    { 2, "System maintenance will occur on March 5th, 2026 from 2 AM to 4 AM UTC.", new DateTime(2026, 3, 4, 9, 30, 0, 0, DateTimeKind.Utc), 1, true, null, "System Maintenance" },
                    { 3, "Please note that all assignments must be submitted before the deadline.", new DateTime(2026, 3, 4, 10, 0, 0, 0, DateTimeKind.Utc), 2, true, 3, "Assignment Deadline Reminder" }
                });

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "NoteID", "Content", "CreatedAt", "IsPinned", "Title", "UpdatedAt", "UserID" },
                values: new object[,]
                {
                    { 1, "Study at least 1 hour/day for mathematics. Focus on algebra and geometry.", new DateTime(2026, 3, 4, 8, 0, 0, 0, DateTimeKind.Utc), true, "Study Plan", null, 3 },
                    { 2, "Review Newton's laws and kinematics before the next class.", new DateTime(2026, 3, 4, 12, 0, 0, 0, DateTimeKind.Utc), false, "Physics Notes", null, 3 },
                    { 3, "Learn 10 new vocabulary words every day for the essay assignment.", new DateTime(2026, 3, 4, 14, 0, 0, 0, DateTimeKind.Utc), false, "English Vocabulary", null, 4 },
                    { 4, "Prepare lab equipment and read the experiment procedure carefully.", new DateTime(2026, 3, 4, 15, 30, 0, 0, DateTimeKind.Utc), true, "Chemistry Lab Preparation", null, 5 }
                });

            migrationBuilder.InsertData(
                table: "StudentAssignmentStatus",
                columns: new[] { "ID", "AssignmentID", "Score", "StudentID", "SubmittedAt" },
                values: new object[,]
                {
                    { 1, 1, 9.0, 3, new DateTime(2026, 3, 3, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, 8.5, 4, new DateTime(2026, 3, 3, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, 13.0, 3, new DateTime(2026, 3, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 5, 18.0, 4, new DateTime(2026, 3, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 6, 11.0, 5, new DateTime(2026, 3, 4, 11, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "AssignmentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "AssignmentID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "EnrollmentID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "NewsID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "NewsID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "NewsID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "NoteID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "NoteID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "NoteID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Notes",
                keyColumn: "NoteID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "StudentAssignmentStatus",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StudentAssignmentStatus",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "StudentAssignmentStatus",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "StudentAssignmentStatus",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "StudentAssignmentStatus",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "AssignmentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "AssignmentID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "AssignmentID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "AssignmentID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "SubjectID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "SubjectID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "SubjectID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "SubjectID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: 2);
        }
    }
}
