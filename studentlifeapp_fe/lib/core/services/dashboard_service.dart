import 'dart:convert';
import 'package:http/http.dart' as http;
import '../constants/api_constants.dart';

class DashboardService {
  final http.Client _client = http.Client();

  Future<Map<String, dynamic>> getStudentDashboard(int userId) async {
    try {
      final uri = Uri.parse(ApiConstants.studentDashboard(userId));
      final res = await _client.get(uri);
      
      if (res.statusCode == 200) {
        return jsonDecode(res.body);
      } else if (res.statusCode == 404) {
        throw Exception('Dashboard data not found. Please check if the user exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to load student dashboard (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<Map<String, dynamic>> getTeacherDashboard(int userId) async {
    try {
      final uri = Uri.parse(ApiConstants.teacherDashboard(userId));
      final res = await _client.get(uri);
      
      if (res.statusCode == 200) {
        return jsonDecode(res.body);
      } else if (res.statusCode == 404) {
        throw Exception('Dashboard data not found. Please check if the user exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to load teacher dashboard (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<List<dynamic>> getPerformanceTrend(int userId) async {
    try {
      final uri = Uri.parse(ApiConstants.performanceTrend(userId));
      final res = await _client.get(uri);
      
      if (res.statusCode == 200) {
        return jsonDecode(res.body);
      } else if (res.statusCode == 404) {
        throw Exception('Performance data not found. Please check if the user exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to load performance trend (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }
}
