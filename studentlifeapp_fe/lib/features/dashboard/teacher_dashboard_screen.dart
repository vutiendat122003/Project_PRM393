import 'package:flutter/material.dart';
import '../../core/services/dashboard_service.dart';

class TeacherDashboardScreen extends StatefulWidget {
  final int userId;
  const TeacherDashboardScreen({super.key, required this.userId});

  @override
  State<TeacherDashboardScreen> createState() => _TeacherDashboardScreenState();
}

class _TeacherDashboardScreenState extends State<TeacherDashboardScreen> {
  final _service = DashboardService();
  Map<String, dynamic>? _data;
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() { _loading = true; _error = null; });
    try {
      final data = await _service.getTeacherDashboard(widget.userId);
      setState(() { _data = data; _loading = false; });
    } catch (e) {
      setState(() { _error = e.toString(); _loading = false; });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F6FA),
      appBar: AppBar(
        title: const Text('Teacher Dashboard'),
        backgroundColor: Colors.indigo,
        foregroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(icon: const Icon(Icons.refresh), onPressed: _load),
        ],
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? _buildError()
              : _buildBody(),
    );
  }

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

  Widget _buildBody() {
    final teacher = _data!['teacher'];
    final stats = _data!['overallStatistics'];
    final subjects = (_data!['subjectOverview'] as List);
    final recentAssignments = (_data!['recentAssignments'] as List);
    final pending = (_data!['pendingSubmissions'] as List);

    return RefreshIndicator(
      onRefresh: _load,
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildTeacherHeader(teacher),
            const SizedBox(height: 16),
            _buildStatsGrid(stats),
            const SizedBox(height: 20),
            _buildSectionTitle('My Subjects'),
            const SizedBox(height: 8),
            ...subjects.map((s) => _buildSubjectCard(s)),
            const SizedBox(height: 20),
            _buildSectionTitle('Recent Assignments'),
            const SizedBox(height: 8),
            recentAssignments.isEmpty
                ? _buildEmptyCard('No assignments yet')
                : Column(children: recentAssignments.map((a) => _buildAssignmentCard(a)).toList()),
            const SizedBox(height: 20),
            _buildSectionTitle('Pending Grading (${pending.length})'),
            const SizedBox(height: 8),
            pending.isEmpty
                ? _buildEmptyCard('All submissions graded!')
                : Column(children: pending.map((p) => _buildPendingCard(p)).toList()),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildTeacherHeader(Map teacher) => Container(
    padding: const EdgeInsets.all(16),
    decoration: BoxDecoration(
      gradient: const LinearGradient(
        colors: [Colors.indigo, Colors.indigoAccent],
        begin: Alignment.topLeft,
        end: Alignment.bottomRight,
      ),
      borderRadius: BorderRadius.circular(16),
    ),
    child: Row(
      children: [
        CircleAvatar(
          radius: 28,
          backgroundColor: Colors.white24,
          child: Text(
            (teacher['fullName'] as String).substring(0, 1).toUpperCase(),
            style: const TextStyle(fontSize: 24, color: Colors.white, fontWeight: FontWeight.bold),
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(teacher['fullName'], style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white)),
              const SizedBox(height: 4),
              Text(teacher['email'], style: const TextStyle(color: Colors.white70, fontSize: 13)),
              const Text('Teacher', style: TextStyle(color: Colors.white54, fontSize: 12)),
            ],
          ),
        ),
      ],
    ),
  );

  Widget _buildStatsGrid(Map stats) => GridView.count(
    crossAxisCount: 3,
    shrinkWrap: true,
    physics: const NeverScrollableScrollPhysics(),
    crossAxisSpacing: 8,
    mainAxisSpacing: 8,
    childAspectRatio: 1.1,
    children: [
      _buildStatCard('Subjects', '${stats['totalSubjects']}', Icons.book, Colors.indigo),
      _buildStatCard('Students', '${stats['totalStudents']}', Icons.people, Colors.blue),
      _buildStatCard('Assignments', '${stats['totalAssignments']}', Icons.assignment, Colors.purple),
      _buildStatCard('Submissions', '${stats['totalSubmissions']}', Icons.upload_file, Colors.teal),
      _buildStatCard('Graded', '${stats['gradedSubmissions']}', Icons.check_circle, Colors.green),
      _buildStatCard('Pending', '${stats['pendingGrading']}', Icons.pending_actions, Colors.orange),
    ],
  );

  Widget _buildStatCard(String label, String value, IconData icon, Color color) => Container(
    padding: const EdgeInsets.all(10),
    decoration: BoxDecoration(
      color: Colors.white,
      borderRadius: BorderRadius.circular(12),
      boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
    ),
    child: Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Icon(icon, color: color, size: 22),
        const SizedBox(height: 4),
        Text(value, style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: color)),
        Text(label, style: const TextStyle(fontSize: 10, color: Colors.grey), textAlign: TextAlign.center),
      ],
    ),
  );

  Widget _buildSectionTitle(String title) => Text(
    title,
    style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Color(0xFF2D2D2D)),
  );

  Widget _buildSubjectCard(Map subject) => Container(
    margin: const EdgeInsets.only(bottom: 10),
    padding: const EdgeInsets.all(14),
    decoration: BoxDecoration(
      color: Colors.white,
      borderRadius: BorderRadius.circular(12),
      boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
    ),
    child: Row(
      children: [
        Container(
          width: 44,
          height: 44,
          decoration: BoxDecoration(color: Colors.indigo.shade50, borderRadius: BorderRadius.circular(10)),
          child: const Icon(Icons.class_, color: Colors.indigo),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(subject['subjectName'], style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
              const SizedBox(height: 4),
              Text('${subject['credits']} credits · ${subject['enrolledStudents']} students', style: const TextStyle(fontSize: 12, color: Colors.grey)),
            ],
          ),
        ),
        Column(
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text('${subject['totalAssignments']}', style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16, color: Colors.indigo)),
            const Text('assignments', style: TextStyle(fontSize: 10, color: Colors.grey)),
          ],
        ),
      ],
    ),
  );

  Widget _buildAssignmentCard(Map a) {
    final submitted = a['totalSubmissions'] as int;
    final graded = a['gradedSubmissions'] as int;
    final progress = submitted > 0 ? graded / submitted : 0.0;

    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Expanded(child: Text(a['title'], style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13))),
              Text(a['subjectName'], style: const TextStyle(fontSize: 11, color: Colors.indigo)),
            ],
          ),
          const SizedBox(height: 6),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('$graded/$submitted graded', style: const TextStyle(fontSize: 12, color: Colors.grey)),
              Text('Max: ${a['maxScore']}pts', style: const TextStyle(fontSize: 12, color: Colors.grey)),
            ],
          ),
          const SizedBox(height: 6),
          ClipRRect(
            borderRadius: BorderRadius.circular(4),
            child: LinearProgressIndicator(
              value: progress.toDouble(),
              minHeight: 5,
              backgroundColor: Colors.grey.shade200,
              valueColor: const AlwaysStoppedAnimation<Color>(Colors.indigo),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPendingCard(Map p) => Container(
    margin: const EdgeInsets.only(bottom: 8),
    padding: const EdgeInsets.all(12),
    decoration: BoxDecoration(
      color: Colors.white,
      borderRadius: BorderRadius.circular(12),
      border: Border(left: BorderSide(color: Colors.orange, width: 4)),
      boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
    ),
    child: Row(
      children: [
        const Icon(Icons.pending_actions, color: Colors.orange, size: 20),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(p['assignmentTitle'], style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
              const SizedBox(height: 2),
              Text('${p['subjectName']} · Student #${p['studentId']}', style: const TextStyle(fontSize: 12, color: Colors.grey)),
            ],
          ),
        ),
        const Icon(Icons.chevron_right, color: Colors.grey),
      ],
    ),
  );

  Widget _buildEmptyCard(String msg) => Container(
    width: double.infinity,
    padding: const EdgeInsets.all(16),
    decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(12)),
    child: Text(msg, textAlign: TextAlign.center, style: const TextStyle(color: Colors.grey)),
  );
}