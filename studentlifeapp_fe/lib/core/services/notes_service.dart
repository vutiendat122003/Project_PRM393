import 'dart:convert';
import 'package:http/http.dart' as http;
import '../constants/api_constants.dart';

class NotesService {
  Future<List<dynamic>> getNotes(int userId) async {
    final res = await http.get(Uri.parse(ApiConstants.notes(userId)));
    if (res.statusCode == 200) return jsonDecode(res.body);
    throw Exception('Failed to load notes');
  }

  Future<List<dynamic>> searchNotes(int userId, String query) async {
    final res = await http.get(Uri.parse(ApiConstants.searchNotes(userId, query)));
    if (res.statusCode == 200) return jsonDecode(res.body);
    throw Exception('Failed to search notes');
  }

  Future<Map<String, dynamic>> createNote({
    required int userId,
    required String title,
    required String content,
    bool isPinned = false,
  }) async {
    final res = await http.post(
      Uri.parse(ApiConstants.createNote()),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'userId': userId,
        'title': title,
        'content': content,
        'isPinned': isPinned,
      }),
    );
    if (res.statusCode == 201) return jsonDecode(res.body);
    throw Exception('Failed to create note');
  }

  Future<Map<String, dynamic>> updateNote({
    required int noteId,
    required int userId,
    String? title,
    String? content,
    bool? isPinned,
  }) async {
    final res = await http.put(
      Uri.parse(ApiConstants.updateNote(noteId)),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'userId': userId,
        if (title != null) 'title': title,
        if (content != null) 'content': content,
        if (isPinned != null) 'isPinned': isPinned,
      }),
    );
    if (res.statusCode == 200) return jsonDecode(res.body);
    throw Exception('Failed to update note');
  }

  Future<void> deleteNote(int noteId, int userId) async {
    final res = await http.delete(Uri.parse(ApiConstants.deleteNote(noteId, userId)));
    if (res.statusCode != 200) throw Exception('Failed to delete note');
  }

  Future<void> togglePin(int noteId, int userId) async {
    final res = await http.patch(Uri.parse(ApiConstants.togglePin(noteId, userId)));
    if (res.statusCode != 200) throw Exception('Failed to toggle pin');
  }
}