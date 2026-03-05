import 'dart:convert';
import 'package:http/http.dart' as http;
import '../constants/api_constants.dart';

class DashboardService {
  Future<Map<String, dynamic>> getStudentDashboard(int userId) async {
    final res = await http.get(Uri.parse(ApiConstants.studentDashboard(userId)));
    if (res.statusCode == 200) return jsonDecode(res.body);
    throw Exception('Failed to load student dashboard');
  }

  Future<Map<String, dynamic>> getTeacherDashboard(int userId) async {
    final res = await http.get(Uri.parse(ApiConstants.teacherDashboard(userId)));
    if (res.statusCode == 200) return jsonDecode(res.body);
    throw Exception('Failed to load teacher dashboard');
  }

  Future<List<dynamic>> getPerformanceTrend(int userId) async {
    final res = await http.get(Uri.parse(ApiConstants.performanceTrend(userId)));
    if (res.statusCode == 200) return jsonDecode(res.body);
    throw Exception('Failed to load performance trend');
  }
}