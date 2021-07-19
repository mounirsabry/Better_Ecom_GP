-- do not forget to use the specific database you want to execute the script on.

CREATE TABLE IF NOT EXISTS department (
    department_code CHAR(2),
    department_name VARCHAR(255),
    PRIMARY KEY (department_code)
);

CREATE TABLE IF NOT EXISTS system_user (
    system_user_id INT,
    user_password VARCHAR(100) NULL,
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    address VARCHAR(255) NOT NULL,
    phone_number VARCHAR(255),
    mobile_number VARCHAR(255),
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
    department_code CHAR(2),
    high_school_type VARCHAR(255) NOT NULL,
    entrance_year INT NOT NULL,
    gpa DOUBLE,
    academic_year INT,
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
    department_code CHAR(2),
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
        
CREATE TABLE IF NOT EXISTS student_department_priority_list (
    student_id INT,
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
    course_code VARCHAR(20) NOT NULL,
    department_code CHAR(2),
    course_name VARCHAR(255) NOT NULL,
    academic_year INT,
    course_description TEXT,
    is_read_only BOOL DEFAULT FALSE,
    is_archived BOOL DEFAULT FALSE,
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
    CONSTRAINT course_instance_code_year_term_combination_unique UNIQUE (course_code , course_year , course_term),
    CONSTRAINT course_instance_course_code FOREIGN KEY (course_code)
        REFERENCES course (course_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (instance_id)
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

CREATE TABLE IF NOT EXISTS student_course_registration (
	registration_instance_id INT AUTO_INCREMENT,
    student_id INT NOT NULL,
    course_instance_id INT NOT NULL,
    registration_date DATE NOT NULL,
    student_course_status ENUM('Undertaking', 'Passed', 'Failed'),
    CONSTRAINT student_course_instance_combination_unique UNIQUE (student_id, course_instance_id),
    CONSTRAINT student_course_registration_student_id FOREIGN KEY (student_id)
		REFERENCES student (student_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT student_course_registration_instance_id FOREIGN KEY (course_instance_id)
		REFERENCES course_instance (instance_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
	PRIMARY KEY (registration_instance_id)
);

INSERT INTO department VALUES ('GE', 'General');
INSERT INTO department VALUES ('CS', 'Computer Science');
INSERT INTO department VALUES ('IS', 'Information Systems');
INSERT INTO department VALUES ('IT', 'Information Technology');
INSERT INTO department VALUES ('DS', 'Decision Support');
INSERT INTO department VALUES ('AI', 'Artifical Intelligence');

INSERT INTO system_user 
VALUES (11, '$2a$11$t6dVz2J.GvNnYanfS1aTQ.vZk72WkIeHXWVGX8t9IfLyxF8x5qTh2', 'Default Admin', 'admin@email.com', 'faculty', NULL, NULL, 'Egyptian', 2000,
'1990-01-01', 'Male', 'Default Admin, should be removed on release.');
INSERT INTO admin_user VALUES (11);

INSERT INTO system_user 
VALUES (20210001, '$2a$11$t6dVz2J.GvNnYanfS1aTQ.vZk72WkIeHXWVGX8t9IfLyxF8x5qTh2', 'Dummy Student', 'student@email.com', 'Unkonwn Location', NULL, NULL, 'Egyptian', 2001,
'1999-01-01', 'Male', 'Dummy Student, should be removed on release.');
INSERT INTO student VALUES (20210001, 'GE', 'Governmental', 2017, NULL, 4);

INSERT INTO system_user
VALUES (31, '$2a$11$t6dVz2J.GvNnYanfS1aTQ.vZk72WkIeHXWVGX8t9IfLyxF8x5qTh2', 'Dummy Instructor', 'instructor@email.com', 'Unknown Location', NULL, NULL, 'Egyptian', 2002,
'1975-01-01', 'Male', 'Dummy Instructor, should be removed on release.');
INSERT INTO instructor VALUES (31, 'GE', 'Cairo University', '1997', 'Office hours Sunday and Thursday on my office after the lectures');

INSERT INTO course
VALUES ('GE101', 'GE', 'Math 1', 1, 'First course of math', FALSE, FALSE);

INSERT INTO course_instance
VALUES (NULL, 'GE101', 2021, 'First', 3); 

INSERT INTO course_department_applicability
VALUES ('GE101', 'GE');

