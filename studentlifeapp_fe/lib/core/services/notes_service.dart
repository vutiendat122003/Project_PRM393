import 'dart:convert';
import 'package:http/http.dart' as http;
import '../constants/api_constants.dart';

class NotesService {
  final http.Client _client = http.Client();

  Future<List<dynamic>> getNotes(int userId) async {
    try {
      final uri = Uri.parse(ApiConstants.notes(userId));
      final res = await _client.get(uri);
      
      if (res.statusCode == 200) {
        return jsonDecode(res.body);
      } else if (res.statusCode == 404) {
        throw Exception('Notes not found. Please check if the user exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to load notes (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<List<dynamic>> searchNotes(int userId, String query) async {
    try {
      final uri = Uri.parse(ApiConstants.searchNotes(userId, query));
      final res = await _client.get(uri);
      
      if (res.statusCode == 200) {
        return jsonDecode(res.body);
      } else if (res.statusCode == 404) {
        throw Exception('Search failed. Please check if the user exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to search notes (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<Map<String, dynamic>> createNote({
    required int userId,
    required String title,
    required String content,
    bool isPinned = false,
  }) async {
    try {
      final uri = Uri.parse(ApiConstants.createNote());
      final res = await _client.post(
        uri,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'userId': userId,
          'title': title,
          'content': content,
          'isPinned': isPinned,
        }),
      );
      
      if (res.statusCode == 201) {
        return jsonDecode(res.body);
      } else if (res.statusCode == 400) {
        throw Exception('Invalid note data. Please check your input.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to create note (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<Map<String, dynamic>> updateNote({
    required int noteId,
    required int userId,
    String? title,
    String? content,
    bool? isPinned,
  }) async {
    try {
      final uri = Uri.parse(ApiConstants.updateNote(noteId));
      final res = await _client.put(
        uri,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'userId': userId,
          if (title != null) 'title': title,
          if (content != null) 'content': content,
          if (isPinned != null) 'isPinned': isPinned,
        }),
      );
      
      if (res.statusCode == 200) {
        return jsonDecode(res.body);
      } else if (res.statusCode == 400) {
        throw Exception('Invalid note data. Please check your input.');
      } else if (res.statusCode == 404) {
        throw Exception('Note not found. Please check if the note exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to update note (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<void> deleteNote(int noteId, int userId) async {
    try {
      final uri = Uri.parse(ApiConstants.deleteNote(noteId, userId));
      final res = await _client.delete(uri);
      
      if (res.statusCode == 200) {
        return;
      } else if (res.statusCode == 404) {
        throw Exception('Note not found. Please check if the note exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to delete note (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }

  Future<void> togglePin(int noteId, int userId) async {
    try {
      final uri = Uri.parse(ApiConstants.togglePin(noteId, userId));
      final res = await _client.patch(uri);
      
      if (res.statusCode == 200) {
        return;
      } else if (res.statusCode == 404) {
        throw Exception('Note not found. Please check if the note exists.');
      } else if (res.statusCode == 500) {
        throw Exception('Server error. Please try again later.');
      } else {
        throw Exception('Failed to toggle pin (Status: ${res.statusCode})');
      }
    } on http.ClientException catch (e) {
      throw Exception('Network error: ${e.message}\n\nPlease check:\n1. Your internet connection\n2. The API server is running at ${ApiConstants.baseUrl}\n3. The server IP address is correct for your device/emulator');
    } on Exception catch (e) {
      throw Exception('Unexpected error: $e');
    }
  }
}
