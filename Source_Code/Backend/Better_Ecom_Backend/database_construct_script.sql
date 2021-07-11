use 90z95MrFle;

CREATE TABLE IF NOT EXISTS system_user (
    system_user_id INT,
    user_password VARCHAR(60) DEFAULT (NULL),
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
    high_school_type VARCHAR(255) NOT NULL,
    entrance_year INT NOT NULL, 
    gpa FLOAT,
    department VARCHAR(255),
    academic_year INT,
    CONSTRAINT student_system_user_id FOREIGN KEY (student_id)
        REFERENCES system_user (system_user_id)
        ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY (student_id)
);

CREATE TABLE IF NOT EXISTS instructor (
    instructor_id INT,
    university VARCHAR(255) NOT NULL,
    graduation_year INT NOT NULL,
    contact_info VARCHAR(255),
    CONSTRAINT instructor_system_user_id FOREIGN KEY (instructor_id)
        REFERENCES system_user (system_user_id)
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

INSERT INTO system_user values (11, "1111", "Default Admin", "admin@email.com", "faculty", NULL, NULL, "Egyptian", 2000, "1990-01-01", "Male", "Default Admin, should be removed on release.");
INSERT INTO admin_user values (11);

INSERT INTO system_user values (20210001, "1111", "Dummy Student", "student@email.com", "Unkonwn Location", NULL, NULL, "Egyptian", 2001, "1999-01-01", "Male", "Dummy Student, should be removed on release."); 
INSERT INTO student values (20210001, "Governmental", "1999", NULL, "General", 4);

INSERT INTO system_user values (31, "1111", "Dummy Instructor", "instructor@email.com", "Unknown Location", NULL, NULL, "Egyptian", 2002, "1975-01-01", "Male", "Dummy Instructor, should be removed on release.");
INSERT INTO instructor values (31, "Cairo University", "1997", "Office hours Sunday and Thursday on my office after the lectures");