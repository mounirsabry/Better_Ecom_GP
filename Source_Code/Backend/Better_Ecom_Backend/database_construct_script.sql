-- do not forget to use the specific database you want to execute the script on.

CREATE TABLE IF NOT EXISTS department (
    department_code CHAR(2),
    department_name VARCHAR(255) NOT NULL,
    PRIMARY KEY (department_code)
);

CREATE TABLE IF NOT EXISTS system_user (
    system_user_id INT,
    user_password VARCHAR(100) NULL,
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NULL DEFAULT "",
    address VARCHAR(255) NOT NULL,
    phone_number VARCHAR(255) NULL DEFAULT "",
    mobile_number VARCHAR(255) NULL DEFAULT "",
    nationality VARCHAR(255) NOT NULL,
    national_id VARCHAR(255) NOT NULL,
    birth_date DATE NOT NULL,
    gender VARCHAR(255) NOT NULL,
    additional_info TEXT,
    CONSTRAINT nationality_national_id_combination_unique UNIQUE (nationality , national_id),
    PRIMARY KEY (system_user_id)
);

CREATE TABLE IF NOT EXISTS student (
    student_id INT,
    department_code CHAR(2) NOT NULL,
    high_school_type VARCHAR(255) NOT NULL,
    entrance_year INT NOT NULL,
    gpa DOUBLE NULL DEFAULT NULL,
    academic_year INT NOT NULL,
    CONSTRAINT student_system_user_id FOREIGN KEY (student_id)
        REFERENCES system_user (system_user_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT student_department FOREIGN KEY (department_code)
        REFERENCES department (department_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (student_id)
);

CREATE TABLE IF NOT EXISTS instructor (
    instructor_id INT,
    department_code CHAR(2) NOT NULL,
    university VARCHAR(255) NOT NULL,
    graduation_year INT NOT NULL,
    contact_info TEXT,
    CONSTRAINT instructor_system_user_id FOREIGN KEY (instructor_id)
        REFERENCES system_user (system_user_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT instructor_department FOREIGN KEY (department_code)
        REFERENCES department (department_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (instructor_id)
);

CREATE TABLE IF NOT EXISTS admin_user (
    admin_user_id INT,
    CONSTRAINT admin_user_system_user_id FOREIGN KEY (admin_user_id)
        REFERENCES system_user (system_user_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (admin_user_id)
);
        
CREATE TABLE IF NOT EXISTS student_department_priority (
    student_id INT NOT NULL,
    department_code CHAR(2) NOT NULL,
    priority INT NOT NULL,
    CONSTRAINT priority_list_student_id FOREIGN KEY (student_id)
        REFERENCES student (student_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT priority_list_department_code FOREIGN KEY (department_code)
        REFERENCES department (department_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (student_id , department_code)
);

CREATE TABLE IF NOT EXISTS course (
    course_code VARCHAR(20),
    department_code CHAR(2) NULL,
    course_name VARCHAR(255) NOT NULL,
    academic_year INT NULL,
    course_description TEXT,
    is_archived BOOL NOT NULL DEFAULT FALSE,
    CONSTRAINT course_department_code FOREIGN KEY (department_code)
        REFERENCES department (department_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (course_code)
);

CREATE TABLE IF NOT EXISTS course_instance (
    instance_id INT AUTO_INCREMENT,
    course_code VARCHAR(20) NOT NULL,
    course_year INT NOT NULL,
    course_term ENUM('First', 'Second', 'Summer', 'Other') NOT NULL,
    credit_hours INT NOT NULL DEFAULT 0,
    is_read_only BOOL NOT NULL DEFAULT FALSE,
    is_closed_for_registration BOOL NOT NULL DEFAULT FALSE,
    CONSTRAINT course_instance_code_year_term_combination_unique UNIQUE (course_code , course_year , course_term),
    CONSTRAINT course_instance_course_code FOREIGN KEY (course_code)
        REFERENCES course (course_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (instance_id)
);

CREATE TABLE IF NOT EXISTS course_prerequisite (
    course_code VARCHAR(20) NOT NULL,
    prerequisite_course_code VARCHAR(20) NOT NULL,
    CONSTRAINT course_prerequisite_course_code FOREIGN KEY (course_code)
        REFERENCES course (course_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT course_prerequisite_prerequisite_course_code FOREIGN KEY (prerequisite_course_code)
        REFERENCES course (course_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (course_code , prerequisite_course_code)
);

CREATE TABLE IF NOT EXISTS course_department_applicability (
    course_code VARCHAR(20) NOT NULL,
    department_code CHAR(2) NOT NULL,
    CONSTRAINT course_department_applicability_course_code FOREIGN KEY (course_code)
        REFERENCES course (course_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT course_department_applicability_department_code FOREIGN KEY (department_code)
        REFERENCES department (department_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (course_code , department_code)
);

CREATE TABLE IF NOT EXISTS course_instance_late_registration_request (
    request_id INT AUTO_INCREMENT,
    student_id INT NOT NULL,
    course_instance_id INT NOT NULL,
    request_date DATETIME NOT NULL,
    request_status ENUM('Pending_Accept', 'Accepted', 'Rejected') NOT NULL,
    CONSTRAINT late_registration_request_combination_unqiue UNIQUE (student_id , course_instance_id),
    CONSTRAINT late_registration_request_student_id FOREIGN KEY (student_id)
        REFERENCES student (student_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT late_registration_request_course_instance_id FOREIGN KEY (course_instance_id)
        REFERENCES course_instance (instance_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (request_id)
);

CREATE TABLE IF NOT EXISTS student_course_instance_registration (
    registration_id INT AUTO_INCREMENT,
    student_id INT NOT NULL,
    course_instance_id INT NOT NULL,
    registration_date DATETIME NOT NULL,
    student_course_instance_status ENUM('Undertaking', 'Passed', 'Failed') NOT NULL,
    student_course_instance_grade ENUM('APlus', 'A', 'BPlus', 'B', 'CPlus', 'C', 'DPlus', 'D', 'F', 'Not_Specified') NOT NULL DEFAULT 'Not_Specified',
    CONSTRAINT student_course_registration_combination_unique UNIQUE (student_id , course_instance_id),
    CONSTRAINT student_course_registration_student_id FOREIGN KEY (student_id)
        REFERENCES student (student_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT student_course_registration_course_instance_id FOREIGN KEY (course_instance_id)
        REFERENCES course_instance (instance_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (registration_id)
);

CREATE TABLE IF NOT EXISTS instructor_course_instance_registration (
    registration_id INT AUTO_INCREMENT,
    instructor_id INT NOT NULL,
    course_instance_id INT NOT NULL,
    registration_date DATETIME NOT NULL,
    CONSTRAINT instructor_course_registration_combination_unqiue UNIQUE (instructor_id , course_instance_id),
    CONSTRAINT instructor_course_registration_instructor_id FOREIGN KEY (instructor_id)
        REFERENCES instructor (instructor_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT instructor_course_registration_course_instance_id FOREIGN KEY (course_instance_id)
        REFERENCES course_instance (instance_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (registration_id)
);

CREATE TABLE IF NOT EXISTS general_feed (
    feed_id INT AUTO_INCREMENT,
    content TEXT NOT NULL,
    insertion_date DATETIME NOT NULL,
    PRIMARY KEY (feed_id)
);

CREATE TABLE IF NOT EXISTS course_instance_feed (
    feed_id INT AUTO_INCREMENT,
    course_instance_id INT NOT NULL,
    content TEXT NOT NULL,
    insertion_date DATETIME NOT NULL,
    CONSTRAINT course_instance_feed_course_instance_id FOREIGN KEY (course_instance_id)
        REFERENCES course_instance (instance_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (feed_id)
);

CREATE TABLE IF NOT EXISTS attendance_item (
    item_id INT AUTO_INCREMENT,
    course_instance_id INT NOT NULL,
    item_name VARCHAR(100) NOT NULL,
    attendance_type ENUM('Lab', 'Section') NOT NULL,
    attendance_date DATETIME NULL,
    CONSTRAINT attendance_name_unique_across_course_instance UNIQUE (course_instance_id , item_name),
    CONSTRAINT attendance_item_course_instance FOREIGN KEY (course_instance_id)
        REFERENCES course_instance (instance_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (item_id)
);

CREATE TABLE IF NOT EXISTS student_attendance_item_attendance (
    attendance_item_id INT NOT NULL,
    student_id INT NOT NULL,
    attendance_status ENUM('Attended', 'Absent', 'Excused', 'Not_Specified') NOT NULL,
    CONSTRAINT attendance_attendance_item_id FOREIGN KEY (attendance_item_id)
		REFERENCES attendance_item (item_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT attendance_student_id FOREIGN KEY (student_id)
		REFERENCES student (student_id)
		ON UPDATE CASCADE ON DELETE CASCADE,
	PRIMARY KEY (attendance_item_id , student_id)
);

INSERT INTO department VALUES ('GE', 'General');
INSERT INTO department VALUES ('CS', 'Computer Science');
INSERT INTO department VALUES ('IS', 'Information Systems');
INSERT INTO department VALUES ('IT', 'Information Technology');
INSERT INTO department VALUES ('DS', 'Decision Support');
INSERT INTO department VALUES ('AI', 'Artifical Intelligence');

INSERT INTO system_user 
VALUES (11, '$2a$11$t6dVz2J.GvNnYanfS1aTQ.vZk72WkIeHXWVGX8t9IfLyxF8x5qTh2', 'Default Admin', 'admin@email.com', 'faculty', NULL, NULL, 'Egyptian', 1001,
'1990-01-01', 'Male', 'Default Admin, should be removed on release.');
INSERT INTO admin_user VALUES (11);

INSERT INTO system_user 
VALUES (20210001, '$2a$11$t6dVz2J.GvNnYanfS1aTQ.vZk72WkIeHXWVGX8t9IfLyxF8x5qTh2', 'Dummy Student', 'student@email.com', 'Unkonwn Location', NULL, NULL, 'Egyptian', 2001,
'1999-01-01', 'Male', 'Dummy Student, should be removed on release.');
INSERT INTO student VALUES (20210001, 'CS', 'Governmental', 2017, NULL, 4);

INSERT INTO system_user 
VALUES (20210002, '$2a$11$t6dVz2J.GvNnYanfS1aTQ.vZk72WkIeHXWVGX8t9IfLyxF8x5qTh2', 'Dummy Student', 'student2@email.com', 'Unkonwn Location', NULL, NULL, 'Egyptian', 2002,
'2003-01-01', 'Male', 'Dummy Student, should be removed on release.');
INSERT INTO student VALUES (20210002, 'GE', 'Governmental', 2021, NULL, 1);

INSERT INTO system_user
VALUES (31, '$2a$11$t6dVz2J.GvNnYanfS1aTQ.vZk72WkIeHXWVGX8t9IfLyxF8x5qTh2', 'Dummy Instructor', 'instructor@email.com', 'Unknown Location', NULL, NULL, 'Egyptian', 3001,
'1975-01-01', 'Male', 'Dummy Instructor, should be removed on release.');
INSERT INTO instructor VALUES (31, 'GE', 'Cairo University', '1997', 'Office hours Sunday and Thursday on my office after the lectures');

INSERT INTO course
VALUES ('GE101', 'GE', 'Math 1', 1, 'First course of math', FALSE);
INSERT INTO course_department_applicability
VALUES ('GE101', 'GE');
INSERT INTO course_instance
VALUES (NULL, 'GE101', 2017, 'First', 3, TRUE, TRUE); 
INSERT INTO course_instance
VALUES (NULL, 'GE101', 2021, 'First', 3, FALSE, FALSE);

INSERT INTO course
VALUES ('GE102', 'GE', 'English', 1, 'English coures.', FALSE);
INSERT INTO course_department_applicability
VALUES ('GE102', 'GE');
INSERT INTO course_instance
VALUES (NULL, 'GE102', 2021, 'First', 3, FALSE, FALSE);

INSERT INTO course
VALUES ('GE103', 'GE', 'Math 2', 1, 'Second course of math', FALSE);
INSERT INTO course_prerequisite
VALUES ('GE103', 'GE101');
INSERT INTO course_department_applicability
VALUES ('GE103', 'GE');
INSERT INTO course_instance
VALUES (NULL, 'GE103', 2021, 'Second', 3, FALSE, FALSE); 

INSERT INTO course
VALUES ('GE104', 'GE', 'Technical Writing', 1, 'Technical Writing course.', FALSE);
INSERT INTO course_department_applicability
VALUES ('GE104', 'GE');
INSERT INTO course_instance
VALUES (NULL, 'GE104', 2021, 'Second', 3, FALSE, FALSE);

INSERT INTO course
VALUES ('GE105', 'GE', 'Programming 1', 1, 'First course of programming.', FALSE);
INSERT INTO course_department_applicability
VALUES ('GE105', 'GE');
INSERT INTO course_instance
VALUES (NULL, 'GE105', 2021, 'Second', 3, FALSE, FALSE);

INSERT INTO course
VALUES ('GE201', 'GE', 'Programming 2', 2, 'Second course of programming, OOP course.', FALSE);
INSERT INTO course_prerequisite
VALUES ('GE201', 'GE105');
INSERT INTO course_department_applicability
VALUES ('GE201', 'GE');
INSERT INTO course_instance
VALUES (NULL, 'GE201', 2017, 'First', 3, TRUE, TRUE);
INSERT INTO course_instance
VALUES (NULL, 'GE201', 2021, 'First', 3, FALSE, FALSE);

INSERT INTO course
VALUES ('GE202', 'GE', 'Data Communication', 2, 'Basics of IT.', FALSE);
INSERT INTO course_department_applicability
VALUES ('GE202', 'GE');

INSERT INTO course
VALUES ('GE203', 'GE', 'Data Structures', 2, 'Fundamentals of data structures.', FALSE);
INSERT INTO course_department_applicability
VALUES ('GE203', 'GE');
INSERT INTO course_instance
VALUES (NULL, 'GE203', 2017, 'First', 3, TRUE, TRUE);
INSERT INTO course_instance
VALUES (NULL, 'GE203', 2021, 'First', 3, FALSE, FALSE);

INSERT INTO course
VALUES ('GE204', 'GE', 'Databases 1', 2, 'General databases course.', FALSE);
INSERT INTO course_department_applicability
VALUES ('GE204', 'GE');

INSERT INTO course
VALUES ('CS301', 'CS', 'Operating Systems 1', 3, 'General operating systems course.', FALSE);
INSERT INTO course_department_applicability 
VALUES ('CS301', 'GE');
INSERT INTO course_department_applicability
VALUES ('CS301', 'CS');

INSERT INTO course
VALUES ('CS302', 'CS', 'Software Engineering 1', 3, 'General software engineering course.', FALSE);
INSERT INTO course_prerequisite
VALUES ('CS302', 'GE201');
INSERT INTO course_department_applicability 
VALUES ('CS302', 'GE');
INSERT INTO course_department_applicability
VALUES ('CS302', 'CS');

INSERT INTO course
VALUES ('CS303', 'CS', 'Parallel Processing', 3, 'Introduction to multi-core and multi-thread programming.', FALSE);
INSERT INTO course_department_applicability 
VALUES ('CS303', 'GE');
INSERT INTO course_department_applicability
VALUES ('CS303', 'CS');

INSERT INTO course
VALUES ('CS304', 'CS', 'Software Engineering 2', 3, 'Second course for software engineering.', FALSE);
INSERT INTO course_prerequisite
VALUES ('CS304', 'CS302');
INSERT INTO course_department_applicability 
VALUES ('CS304', 'GE');
INSERT INTO course_department_applicability
VALUES ('CS304', 'CS');

INSERT INTO course
VALUES ('CS404', 'CS', 'ERP', 4, 'Data science', FALSE);
INSERT INTO course_department_applicability 
VALUES ('CS404', 'GE');
INSERT INTO course_department_applicability
VALUES ('CS404', 'CS');
INSERT INTO course_department_applicability
VALUES ('CS404', 'IS');
INSERT INTO course_department_applicability
VALUES ('CS404', 'IT');
INSERT INTO course_department_applicability
VALUES ('CS404', 'DS');
INSERT INTO course_instance
VALUES (NULL, 'CS404', 2021, 'Second', 3, FALSE, FALSE);

INSERT INTO student_course_instance_registration
VALUES (NULL, '20210001', 1, '2017-10-01', 'Passed', DEFAULT);
INSERT INTO student_course_instance_registration
VALUES (NULL, '20210001', 11, '2021-02-01', 'Undertaking', DEFAULT);

INSERT INTO student_course_instance_registration
VALUES (NULL, '20210002', 1, '2021-10-01', 'Undertaking', DEFAULT);
