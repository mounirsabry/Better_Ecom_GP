use 90z95MrFle;

ALTER TABLE student DROP INDEX student_department;
ALTER TABLE instructor DROP INDEX instructor_department;
DROP TABLE IF EXISTS student_department_wish_list;
DROP TABLE IF EXISTS department;
DROP TABLE IF EXISTS student;
DROP TABLE IF EXISTS instructor;
DROP TABLE IF EXISTS admin_user;
DROP TABLE IF EXISTS system_user;