import 'package:flutter/material.dart';
import 'screens/tt01_insert_score_screen.dart';
import 'screens/tt02_get_score_screen.dart';
import 'screens/tt03_gpa_screen.dart';
import 'features/dashboard/student_dashboard_screen.dart';
import 'features/dashboard/teacher_dashboard_screen.dart';
import 'features/notes/notes_screen.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Student Life App',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: const MenuScreen(),
    );
  }
}

class MenuScreen extends StatelessWidget {
  const MenuScreen({super.key});

  // Hardcoded for testing — same pattern as teammates (studentId: 1)
  static const int _testStudentId = 1;
  static const int _testTeacherId = 2;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Student Life App'),
        backgroundColor: Colors.deepPurple,
        foregroundColor: Colors.white,
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: 10),

              // ── Score section ──
              _SectionHeader(title: 'Score Management'),
              const SizedBox(height: 8),
              _MenuButton(
                label: 'TT-01 Insert Score',
                icon: Icons.add_chart,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const TT01InsertScoreScreen())),
              ),
              const SizedBox(height: 10),
              _MenuButton(
                label: 'TT-02 Get Scores',
                icon: Icons.list_alt,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const TT02GetScoreScreen(studentId: _testStudentId))),
              ),
              const SizedBox(height: 10),
              _MenuButton(
                label: 'TT-03 GPA',
                icon: Icons.school,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const TT03GPAScreen(studentId: _testStudentId))),
              ),

              const SizedBox(height: 24),

              // ── Dashboard section ──
              _SectionHeader(title: 'Dashboards'),
              const SizedBox(height: 8),
              _MenuButton(
                label: 'Student Dashboard',
                icon: Icons.dashboard,
                color: Colors.deepPurple,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const StudentDashboardScreen(userId: _testStudentId))),
              ),
              const SizedBox(height: 10),
              _MenuButton(
                label: 'Teacher Dashboard',
                icon: Icons.cast_for_education,
                color: Colors.indigo,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const TeacherDashboardScreen(userId: _testTeacherId))),
              ),

              const SizedBox(height: 24),

              // ── Notes section ──
              _SectionHeader(title: 'Notes'),
              const SizedBox(height: 8),
              _MenuButton(
                label: 'My Notes',
                icon: Icons.note_alt,
                color: Colors.teal,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const NotesScreen(userId: _testStudentId))),
              ),

              const SizedBox(height: 30),
            ],
          ),
        ),
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  final String title;
  const _SectionHeader({required this.title});

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: Colors.grey, letterSpacing: 0.5),
    );
  }
}

class _MenuButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final Color color;
  final VoidCallback onTap;

  const _MenuButton({
    required this.label,
    required this.icon,
    required this.onTap,
    this.color = Colors.deepPurple,
  });

  @override
  Widget build(BuildContext context) {
    return ElevatedButton.icon(
      onPressed: onTap,
      icon: Icon(icon),
      label: Text(label),
      style: ElevatedButton.styleFrom(
        backgroundColor: color,
        foregroundColor: Colors.white,
        padding: const EdgeInsets.symmetric(vertical: 14),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        alignment: Alignment.centerLeft,
      ),
    );
  }
}