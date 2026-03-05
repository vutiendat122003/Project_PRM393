import 'package:flutter/material.dart';
import '../../core/services/notes_service.dart';
import 'note_edit_screen.dart';

class NotesScreen extends StatefulWidget {
  final int userId;
  const NotesScreen({super.key, required this.userId});

  @override
  State<NotesScreen> createState() => _NotesScreenState();
}

class _NotesScreenState extends State<NotesScreen> {
  final _service = NotesService();
  final _searchController = TextEditingController();
  List<dynamic> _notes = [];
  bool _loading = true;
  String? _error;
  bool _searching = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() { _loading = true; _error = null; _searching = false; });
    _searchController.clear();
    try {
      final notes = await _service.getNotes(widget.userId);
      setState(() { _notes = notes; _loading = false; });
    } catch (e) {
      setState(() { _error = e.toString(); _loading = false; });
    }
  }

  Future<void> _search(String query) async {
    if (query.trim().isEmpty) { _load(); return; }
    setState(() { _loading = true; _searching = true; });
    try {
      final notes = await _service.searchNotes(widget.userId, query.trim());
      setState(() { _notes = notes; _loading = false; });
    } catch (e) {
      setState(() { _error = e.toString(); _loading = false; });
    }
  }

  Future<void> _togglePin(Map note) async {
    try {
      await _service.togglePin(note['noteId'], widget.userId);
      _load();
    } catch (e) {
      _showSnack('Failed to update pin status');
    }
  }

  Future<void> _delete(Map note) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Delete Note'),
        content: Text('Delete "${note['title']}"?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Cancel')),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Delete', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );
    if (confirm != true) return;
    try {
      await _service.deleteNote(note['noteId'], widget.userId);
      _showSnack('Note deleted');
      _load();
    } catch (e) {
      _showSnack('Failed to delete note');
    }
  }

  void _showSnack(String msg) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
  }

  Future<void> _openEdit({Map? note}) async {
    final result = await Navigator.push<bool>(
      context,
      MaterialPageRoute(
        builder: (_) => NoteEditScreen(userId: widget.userId, note: note),
      ),
    );
    if (result == true) _load();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F6FA),
      appBar: AppBar(
        title: const Text('My Notes'),
        backgroundColor: Colors.deepPurple,
        foregroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(icon: const Icon(Icons.refresh), onPressed: _load),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _openEdit(),
        backgroundColor: Colors.deepPurple,
        child: const Icon(Icons.add, color: Colors.white),
      ),
      body: Column(
        children: [
          _buildSearchBar(),
          Expanded(
            child: _loading
                ? const Center(child: CircularProgressIndicator())
                : _error != null
                    ? _buildError()
                    : _notes.isEmpty
                        ? _buildEmpty()
                        : _buildNotesList(),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchBar() => Container(
    color: Colors.deepPurple,
    padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
    child: TextField(
      controller: _searchController,
      onSubmitted: _search,
      onChanged: (v) { if (v.isEmpty) _load(); },
      style: const TextStyle(color: Colors.white),
      decoration: InputDecoration(
        hintText: 'Search notes...',
        hintStyle: const TextStyle(color: Colors.white54),
        filled: true,
        fillColor: Colors.white24,
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(12), borderSide: BorderSide.none),
        prefixIcon: const Icon(Icons.search, color: Colors.white70),
        suffixIcon: _searchController.text.isNotEmpty
            ? IconButton(icon: const Icon(Icons.close, color: Colors.white70), onPressed: _load)
            : null,
        contentPadding: const EdgeInsets.symmetric(vertical: 10),
      ),
    ),
  );

  Widget _buildError() => Center(
    child: Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        const Icon(Icons.error_outline, size: 48, color: Colors.red),
        const SizedBox(height: 12),
        Text(_error!, textAlign: TextAlign.center),
        const SizedBox(height: 12),
        ElevatedButton(onPressed: _load, child: const Text('Retry')),
      ],
    ),
  );

  Widget _buildEmpty() => Center(
    child: Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Icon(Icons.note_alt_outlined, size: 64, color: Colors.grey.shade400),
        const SizedBox(height: 12),
        Text(
          _searching ? 'No notes found' : 'No notes yet\nTap + to create one',
          textAlign: TextAlign.center,
          style: TextStyle(color: Colors.grey.shade500, fontSize: 16),
        ),
      ],
    ),
  );

  Widget _buildNotesList() {
    final pinned = _notes.where((n) => n['isPinned'] == true).toList();
    final unpinned = _notes.where((n) => n['isPinned'] != true).toList();

    return RefreshIndicator(
      onRefresh: _load,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          if (pinned.isNotEmpty) ...[
            _buildListHeader('📌 Pinned'),
            const SizedBox(height: 8),
            ...pinned.map((n) => _buildNoteCard(n)),
            const SizedBox(height: 12),
          ],
          if (unpinned.isNotEmpty) ...[
            if (pinned.isNotEmpty) _buildListHeader('Notes'),
            if (pinned.isNotEmpty) const SizedBox(height: 8),
            ...unpinned.map((n) => _buildNoteCard(n)),
          ],
        ],
      ),
    );
  }

  Widget _buildListHeader(String title) => Text(
    title,
    style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: Colors.grey),
  );

  Widget _buildNoteCard(Map note) {
    final isPinned = note['isPinned'] == true;
    final updatedAt = DateTime.tryParse(note['updatedAt'] ?? '');
    final dateStr = updatedAt != null
        ? '${updatedAt.day}/${updatedAt.month}/${updatedAt.year}'
        : '';

    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: isPinned ? Border.all(color: Colors.deepPurple.shade100, width: 1.5) : null,
        boxShadow: [BoxShadow(color: Colors.black.withOpacity(0.06), blurRadius: 6, offset: const Offset(0, 2))],
      ),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: () => _openEdit(note: note),
        child: Padding(
          padding: const EdgeInsets.all(14),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      note['title'] ?? 'Untitled',
                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  GestureDetector(
                    onTap: () => _togglePin(note),
                    child: Icon(
                      isPinned ? Icons.push_pin : Icons.push_pin_outlined,
                      size: 18,
                      color: isPinned ? Colors.deepPurple : Colors.grey,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 6),
              Text(
                note['content'] ?? '',
                style: const TextStyle(fontSize: 13, color: Colors.black54),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 8),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(dateStr, style: const TextStyle(fontSize: 11, color: Colors.grey)),
                  GestureDetector(
                    onTap: () => _delete(note),
                    child: const Icon(Icons.delete_outline, size: 18, color: Colors.redAccent),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}