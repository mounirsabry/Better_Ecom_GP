use 90z95MrFle;

CREATE TABLE IF NOT EXISTS department (
    department_code CHAR(2),
    department_name VARCHAR(255),
    PRIMARY KEY (department_code)
);

CREATE TABLE IF NOT EXISTS system_user (
    system_user_id INT,
    user_password VARCHAR(60) NULL,
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    address VARCHAR(255) NOT NULL,
    phone_number VARCHAR(255),
    mobile_number VARCHAR(255),
    nationality VARCHAR(255) NOT NULL,
    national_id VARCHAR(255) NOT NULL,
    birth_date DATE NOT NULL,
    gender VARCHAR(255) NOT NULL,
    additional_info VARCHAR(255),
    PRIMARY KEY (system_user_id)
);

CREATE TABLE IF NOT EXISTS student (
    student_id INT,
    department_code CHAR(2),
    high_school_type VARCHAR(255) NOT NULL,
    entrance_year INT NOT NULL,
    gpa FLOAT,
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
    contact_info VARCHAR(255),
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
    CONSTRAINT admin_user__system_user_id FOREIGN KEY (admin_user_id)
        REFERENCES system_user (system_user_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (admin_user_id)
);

ALTER TABLE department
ADD CONSTRAINT head_of_department_instructor_id FOREIGN KEY (head_of_department_id)
        REFERENCES instructor (instructor_id)
        ON UPDATE CASCADE ON DELETE CASCADE;
        
CREATE TABLE IF NOT EXISTS student_department_priority_list (
	student_id INT,
    department_code CHAR(2),
    priority INT NOT NULL,
    CONSTRAINT priority_list_student_id FOREIGN KEY (student_id)
		REFERENCES student (student_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT priority_list_department_code FOREIGN KEY (department_code)
		REFERENCES department (department_code)
        ON UPDATE CASCADE ON DELETE CASCADE,
	PRIMARY KEY (student_id, department_code)
);

INSERT INTO dpeartment VALUES ("GE", "General", NULL);
INSERT INTO department VALUES ("CS", "Computer Science", NULL);
INSERT INTO department VALUES ("IS", "Information Systems", NULL);
INSERT INTO department VALUES ("IT", "Information Technology", NULL);
INSERT INTO department VALUES ("DS", "Decision Support", NULL);
INSERT INTO department VALUES ("AI", "Artifical Intelligence", NULL);

INSERT INTO system_user 
values (11, "1111", "Default Admin", "admin@email.com", "faculty", NULL, NULL, "Egyptian", 2000,
"1990-01-01", "Male", "Default Admin, should be removed on release.");
INSERT INTO admin_user VALUES (11);

INSERT INTO system_user 
values (20210001, "1111", "Dummy Student", "student@email.com", "Unkonwn Location", NULL, NULL, "Egyptian", 2001,
"1999-01-01", "Male", "Dummy Student, should be removed on release.");
INSERT INTO student VALUES (20210001, "Governmental", "1999", NULL, "General", 4);

INSERT INTO system_user
 values (31, "1111", "Dummy Instructor", "instructor@email.com", "Unknown Location", NULL, NULL, "Egyptian", 2002,
"1975-01-01", "Male", "Dummy Instructor, should be removed on release.");
INSERT INTO instructor VALUES (31, "Cairo University", "1997", "Office hours Sunday and Thursday on my office after the lectures");