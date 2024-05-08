## Table of Contents
- [Core Functionalities](#core-functionalities)
  - [Pass Management](#pass-management)
  - [Advanced Functionalities](#advanced-functionalities)
- [Passes](#passes)
  - [Types of Passes](#types-of-passes)
  - [Pass Duration and Management](#pass-duration-and-management)
  - [Reservation System and Dynamic Attendance Adjustments](#reservation-system-and-dynamic-attendance-adjustments)
  - [Adjustments for Missed Classes](#adjustments-for-missed-classes)
  - [Management Features](#management-features)
  - [System Integration and Notifications](#system-integration-and-notifications)

Absolutely! Here’s an updated summary of the key functionalities and features for your dance studio app, reflecting the additional details and new functionalities:

## Core Functionalities
1. **Class Management**
   - Admins can create and update course information including descriptions, instructors, and class schedules aligned with the studio’s operational hours.
   - Classes are assigned to time slots, with the potential for future adjustments in class duration and breaks.

2. **User Management**
   - Admins can manage student profiles and staff accounts, including creating and configuring roles for Desk Persons.

3. **Reservation and Attendance System**
   - Students can reserve spots in classes according to their pass type.
   - Desk Persons and instructors can check attendance, marking students present or absent, directly impacting pass usage calculations.

### Pass Management
1. **Pass Types and Utilization**
   - Various pass options are available, such as passes allowing for 1, 2, 3, or 4 classes per week, and an unlimited pass.
   - Passes have a 4-week validity, automatically extending for holidays, with an option for students to suspend their pass once for up to two weeks.
   - If a student attends more classes than their pass allows in a week, future class entitlements are adjusted accordingly, reducing the number of classes they can attend in later weeks within the pass duration.

2. **Front Desk Operations (New Role: Desk Person)**
   - Desk Persons can verify if a student is eligible to attend an upcoming class and manage pass-related transactions (selling, suspending, adjusting reservations).
   - They can also adjust the use of future class reservations for current classes, managing both standard and extra attendances.

### Advanced Functionalities
1. **Dynamic Pass Adjustment**
   - The system adjusts the number of classes available in future weeks if a student attends more than their weekly quota, ensuring total pass usage remains constant.

2. **Role-Based Access Control**
   - Different access levels for Admins and Desk Persons, ensuring that each user has appropriate permissions for their role’s responsibilities.

3. **Eligibility Checks and Pass Adjustments**
   - Real-time checks for student eligibility for classes, with functionality to handle substitutions using future reservations if a student with a limited pass wishes to attend an unplanned class.

4. **System Notifications and Reporting**
   - Automated notifications to students about pass adjustments and class availability.
   - Detailed reporting for Admins and Desk Persons on pass utilization and class attendance patterns.

## Passes
### Types of Passes
1. **Limited Passes (1, 2, 3, 4, 5 classes per week)**
   - Students with these passes must select specific classes to attend during the validity period of 4 weeks (28 days). This ensures that they are committed to a consistent schedule that matches their dance level and progression.
   - Each pass specifies the number of classes a student can attend each week.

2. **Full Pass (Unlimited classes)**
   - Offers unrestricted access to classes, allowing students to participate in any class according to their preference and availability within the 28-day validity period.

### Pass Duration and Management
- **Holiday Extensions:** Pass durations extend automatically in the event of public holidays, ensuring students don't miss out on classes due to studio closures.
- **One-time Suspension:** Passes can be suspended once for up to two weeks, with the validity period extended accordingly, accommodating unexpected changes in a student's ability to attend classes.

### Reservation System and Dynamic Attendance Adjustments
- **Mandatory Class Selection for Limited Pass Holders:** Students must choose their classes at the time of pass purchase, promoting structured learning paths.
- **Flexibility for Rescheduling in Advance:** If students know they will miss a scheduled class, they can adjust their reservations in advance to attend a different class, allowing them to manage their attendance proactively without missing out on their learning goals.

### Adjustments for Missed Classes
- **Make-up Opportunities:** If a student misses a class unexpectedly, they can use their class credit to attend another class within the next 7 days. This flexibility ensures that students maintain their progress even when their schedules change unexpectedly.
- **Dynamic Class Attendance:** Should students attend more classes than their weekly limit on a limited pass, their remaining class credits for the 28-day period adjust accordingly. This mechanism ensures they can make up for missed classes without disrupting their overall class count for the period.

### Management Features
- **Roles and Permissions:**
   - **Admins** control overall operations including the creation and management of passes, user roles, and studio scheduling.
   - **Desk Person** handles daily check-ins, pass transactions, and assists with reservation adjustments and tracking class attendances.

### System Integration and Notifications
- **Database Management:** Efficiently tracks pass start and expiration dates, number of classes attended, classes remaining, and specific classes booked. This ensures accurate monitoring of student commitments and attendance.
- **Automated Notifications:** Sends reminders and updates to students about class bookings, upcoming class changes, and pass expiration notices.

This system not only promotes consistency and commitment in students' dance education but also provides ample flexibility to adapt to their dynamic schedules. The structure supports continuous engagement and progression in dance training by accommodating both planned and unplanned changes in attendance.****
