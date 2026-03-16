class ApiConstants {
  // Use localhost for Android emulator, 127.0.0.1 for iOS simulator, or your actual server IP
  static const String baseUrl = 'http://localhost:5077'; // Android emulator
  // For iOS simulator, use: 'http://127.0.0.1:5077'
  // For physical device, use your computer's IP address: 'http://YOUR_IP:5077'

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