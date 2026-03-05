import 'package:flutter/material.dart';
import '../../core/services/dashboard_service.dart';

class StudentDashboardScreen extends StatefulWidget {
  final int userId;
  const StudentDashboardScreen({super.key, required this.userId});

  @override
  State<StudentDashboardScreen> createState() => _StudentDashboardScreenState();
}

class _StudentDashboardScreenState extends State<StudentDashboardScreen> {
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
      final data = await _service.getStudentDashboard(widget.userId);
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
        title: const Text('Student Dashboard'),
        backgroundColor: Colors.deepPurple,
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
    final student = _data!['student'];
    final stats = _data!['overallStatistics'];
    final subjects = (_data!['subjectProgress'] as List);
    final recentGrades = (_data!['recentGrades'] as List);
    final upcoming = (_data!['upcomingDeadlines'] as List);

    return RefreshIndicator(
      onRefresh: _load,
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildStudentHeader(student),
            const SizedBox(height: 16),
            _buildStatsRow(stats),
            const SizedBox(height: 20),
            _buildSectionTitle('Subject Progress'),
            const SizedBox(height: 8),
            ...subjects.map((s) => _buildSubjectCard(s)),
            const SizedBox(height: 20),
            _buildSectionTitle('Upcoming Deadlines'),
            const SizedBox(height: 8),
            upcoming.isEmpty
                ? _buildEmptyCard('No upcoming deadlines')
                : Column(children: upcoming.map((d) => _buildDeadlineCard(d)).toList()),
            const SizedBox(height: 20),
            _buildSectionTitle('Recent Grades'),
            const SizedBox(height: 8),
            recentGrades.isEmpty
                ? _buildEmptyCard('No grades yet')
                : Column(children: recentGrades.map((g) => _buildGradeCard(g)).toList()),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildStudentHeader(Map student) => Container(
    padding: const EdgeInsets.all(16),
    decoration: BoxDecoration(
      gradient: const LinearGradient(
        colors: [Colors.deepPurple, Colors.purpleAccent],
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
            (student['fullName'] as String).substring(0, 1).toUpperCase(),
            style: const TextStyle(fontSize: 24, color: Colors.white, fontWeight: FontWeight.bold),
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(student['fullName'], style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white)),
              const SizedBox(height: 4),
              Text(student['email'], style: const TextStyle(color: Colors.white70, fontSize: 13)),
              if (student['studentCode'] != null)
                Text('ID: ${student['studentCode']}', style: const TextStyle(color: Colors.white70, fontSize: 13)),
            ],
          ),
        ),
      ],
    ),
  );

  Widget _buildStatsRow(Map stats) => Row(
    children: [
      _buildStatCard('Subjects', '${stats['totalSubjects']}', Icons.book, Colors.blue),
      const SizedBox(width: 8),
      _buildStatCard('Done', '${stats['completedAssignments']}/${stats['totalAssignments']}', Icons.task_alt, Colors.green),
      const SizedBox(width: 8),
      _buildStatCard('Avg Score', '${stats['averageScore']}', Icons.grade, Colors.orange),
    ],
  );

  Widget _buildStatCard(String label, String value, IconData icon, Color color) => Expanded(
    child: Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
      ),
      child: Column(
        children: [
          Icon(icon, color: color, size: 24),
          const SizedBox(height: 6),
          Text(value, style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: color)),
          Text(label, style: const TextStyle(fontSize: 11, color: Colors.grey)),
        ],
      ),
    ),
  );

  Widget _buildSectionTitle(String title) => Text(
    title,
    style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Color(0xFF2D2D2D)),
  );

  Widget _buildSubjectCard(Map subject) {
    final progress = (subject['completionPercentage'] as num).toDouble() / 100;
    final avg = (subject['averageScore'] as num).toDouble();
    final isPassing = subject['isPassing'] as bool;

    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
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
              Expanded(child: Text(subject['subjectName'], style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14))),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: isPassing ? Colors.green.shade50 : Colors.red.shade50,
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Text(
                  isPassing ? 'Passing' : 'At Risk',
                  style: TextStyle(fontSize: 11, color: isPassing ? Colors.green : Colors.red, fontWeight: FontWeight.w600),
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('${subject['completedAssignments']}/${subject['totalAssignments']} tasks', style: const TextStyle(fontSize: 12, color: Colors.grey)),
              Text('Avg: $avg / 10', style: const TextStyle(fontSize: 12, color: Colors.grey)),
            ],
          ),
          const SizedBox(height: 6),
          ClipRRect(
            borderRadius: BorderRadius.circular(4),
            child: LinearProgressIndicator(
              value: progress,
              minHeight: 6,
              backgroundColor: Colors.grey.shade200,
              valueColor: AlwaysStoppedAnimation<Color>(isPassing ? Colors.deepPurple : Colors.orange),
            ),
          ),
          const SizedBox(height: 4),
          Text('${subject['completionPercentage']}% complete', style: const TextStyle(fontSize: 11, color: Colors.grey)),
        ],
      ),
    );
  }

  Widget _buildDeadlineCard(Map d) {
    final deadline = DateTime.tryParse(d['deadline'] ?? '');
    final daysLeft = deadline != null ? deadline.difference(DateTime.now()).inDays : 0;
    final isUrgent = daysLeft <= 2;

    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.left(color: isUrgent ? Colors.red : Colors.deepPurple, width: 4),
        boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
      ),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(d['title'], style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                const SizedBox(height: 2),
                Text(d['subjectName'], style: const TextStyle(fontSize: 12, color: Colors.grey)),
              ],
            ),
          ),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text(
                daysLeft == 0 ? 'Today!' : '$daysLeft days left',
                style: TextStyle(fontSize: 12, color: isUrgent ? Colors.red : Colors.deepPurple, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 2),
              Icon(d['isGraded'] ? Icons.check_circle : (d['isSubmitted'] ? Icons.upload_file : Icons.pending),
                  size: 16,
                  color: d['isGraded'] ? Colors.green : (d['isSubmitted'] ? Colors.blue : Colors.orange)),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildGradeCard(Map g) {
    final score = (g['scorePercentage'] as num).toDouble();
    final color = score >= 8 ? Colors.green : score >= 5 ? Colors.orange : Colors.red;

    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.all(12),
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
            decoration: BoxDecoration(color: color.withOpacity(0.1), shape: BoxShape.circle),
            child: Center(child: Text('${score.toStringAsFixed(1)}', style: TextStyle(color: color, fontWeight: FontWeight.bold, fontSize: 13))),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(g['assignmentTitle'], style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                const SizedBox(height: 2),
                Text(g['subjectName'], style: const TextStyle(fontSize: 12, color: Colors.grey)),
              ],
            ),
          ),
          Text('${g['score']} / ${g['maxScore']}', style: const TextStyle(fontSize: 12, color: Colors.grey)),
        ],
      ),
    );
  }

  Widget _buildEmptyCard(String msg) => Container(
    width: double.infinity,
    padding: const EdgeInsets.all(16),
    decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(12)),
    child: Text(msg, textAlign: TextAlign.center, style: const TextStyle(color: Colors.grey)),
  );
}