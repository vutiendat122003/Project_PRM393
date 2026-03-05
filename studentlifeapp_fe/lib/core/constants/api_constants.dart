class ApiConstants {
  static const String baseUrl = 'http://10.0.2.2:5077';

  // Dashboard
  static String studentDashboard(int userId) => '$baseUrl/api/dashboard/student/$userId';
  static String teacherDashboard(int userId) => '$baseUrl/api/dashboard/teacher/$userId';
  static String performanceTrend(int userId) => '$baseUrl/api/dashboard/performance-trend/$userId';

  // Notes
  static String notes(int userId) => '$baseUrl/api/notes?userId=$userId';
  static String noteById(int id, int userId) => '$baseUrl/api/notes/$id?userId=$userId';
  static String createNote() => '$baseUrl/api/notes';
  static String updateNote(int id) => '$baseUrl/api/notes/$id';
  static String deleteNote(int id, int userId) => '$baseUrl/api/notes/$id?userId=$userId';
  static String togglePin(int id, int userId) => '$baseUrl/api/notes/$id/pin?userId=$userId';
  static String searchNotes(int userId, String query) =>
      '$baseUrl/api/notes/search?userId=$userId&query=$query';
}