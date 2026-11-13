CREATE DATABASE IF NOT EXISTS holiday_maker;
USE holiday_maker;

create table customers
(
    id           int auto_increment
        primary key,
    first_name   varchar(30)  not null,
    last_name    varchar(30)  not null,
    email        varchar(100) null,
    phone_number varchar(30)  not null
);

create table hotels
(
    id                      int auto_increment
        primary key,
    name                    varchar(30)          not null,
    city                    varchar(30)          not null,
    pool                    tinyint(1) default 0 not null,
    restaurant              tinyint(1) default 0 not null,
    distance_to_beach       int                  not null,
    distance_to_city_center int                  not null,
    rating                  int                  not null,
    constraint city_hotell_name_unique
        unique (name, city),
    constraint hotels_rating_check
        check (`rating` between 1 and 5)
);

create table rooms
(
    id       int auto_increment
        primary key,
    hotel_id int           not null,
    name     varchar(30)   not null,
    capacity int           not null,
    price    decimal(8, 2) not null,
    constraint rooms_hotel_name_unique
        unique (hotel_id, name),
    constraint rooms_hotels_id_fk
        foreign key (hotel_id) references hotels (id),
    constraint rooms_capacity_check
        check (`capacity` in (1, 2, 4))
);

create table bookings
(
    id               int auto_increment
        primary key,
    start_date       date not null,
    customer_id      int  not null,
    room_id          int  not null,
    end_date         date not null,
    number_of_guests int  not null,
    constraint bookings_customers_id_fk
        foreign key (customer_id) references customers (id),
    constraint bookings_rooms_id_fk
        foreign key (room_id) references rooms (id),
    constraint bookings_dates_check
        check (`end_date` > `start_date`),
    constraint number_of_guests_check
        check (`number_of_guests` > 0)
);

INSERT INTO hotels (id, name, city, pool, restaurant, distance_to_beach, distance_to_city_center, rating)
VALUES
  (1, 'Strandhotellet',       'Malmö',  TRUE,  TRUE,   150, 2500, 4),
  (2, 'Hotell Centralen',     'Malmö',  FALSE, TRUE,  3000,  200, 3),
  (3, 'Pensionat Lilla Torg', 'Malmö',  FALSE, FALSE, 2200,  100, 5),
  (4, 'Strandhotellet',       'Båstad', FALSE, TRUE,    50,  900, 5),
  (5, 'Bjärehemmet',          'Båstad', TRUE,  FALSE, 1800,  400, 2);

INSERT INTO rooms (id, hotel_id, name, capacity, price)
VALUES
  (1,  1, '101', 1,  850.00), (2,  1, '102', 2, 1250.00), (3,  1, '103', 2, 1350.00),
  (4,  1, '201', 4, 2200.00), (5,  1, '202', 4, 2400.00),
  (6,  2, '11', 1,  690.00), (7,  2, '12', 2,  990.00), (8,  2, '13', 2,  990.00),
  (9,  2, '14', 4, 1690.00),
  (10, 3, 'A', 1,  750.00), (11, 3, 'B', 2, 1150.00), (12, 3, 'C', 2, 1190.00),
  (13, 4, '1', 2, 1890.00), (14, 4, '2', 2, 1990.00), (15, 4, '3', 4, 2990.00),
  (16, 4, '4', 1, 1290.00), (17, 4, '5', 2, 2090.00),
  (18, 5, '1', 1,  590.00), (19, 5, '2', 2,  890.00), (20, 5, '3', 4, 1490.00);

INSERT INTO customers (id, first_name, last_name, email, phone_number)
VALUES
  (6,  'Anna',      'Lind',   'anna.lind@exempel.se',  '070-111 11 11'),
  (7,  'Erik',      'Berg',   'erik.berg@exempel.se',  '070-222 22 22'),
  (8,  'Sara',      'Holm',   NULL,                    '070-333 33 33'),
  (9,  'Emelianna', 'Rahimi', 'm.rahimi@exempel.se',   '070-444 44 44'),
  (10, 'Lena',      'Ström',  'lena.strom@exempel.se', '070-555 55 55');

INSERT INTO bookings (customer_id, room_id, start_date, end_date, number_of_guests)
VALUES
  (6,  7, '2026-12-10', '2026-12-13', 2),
  (6, 15, '2026-12-20', '2026-12-27', 4),
  (7, 20, '2026-12-20', '2026-12-27', 3),
  (7,  1, '2026-12-01', '2026-12-04', 1),
  (8,  2, '2026-12-05', '2026-12-08', 2),
  (8,  4, '2026-12-23', '2026-12-26', 4),
  (9, 11, '2026-12-12', '2026-12-14', 2),
  (9, 18, '2026-12-02', '2026-12-09', 1),
  (10,13, '2026-12-15', '2026-12-18', 2),
  (10, 9, '2026-12-28', '2026-12-31', 4),
  (6,  3, '2026-12-18', '2026-12-22', 2),
  (7,  6, '2026-12-07', '2026-12-08', 1),
  (8, 10, '2026-12-19', '2026-12-24', 1),
  (9, 16, '2026-12-26', '2026-12-29', 1),
  (10, 5, '2026-12-03', '2026-12-10', 3),
  (6, 12, '2026-12-01', '2026-12-03', 2),
  (7, 14, '2026-12-08', '2026-12-12', 2),
  (8, 17, '2026-12-27', '2026-12-31', 2),
  (9, 19, '2026-12-14', '2026-12-16', 1),
  (10, 7, '2026-12-20', '2026-12-23', 2),
  (6,  2, '2026-12-24', '2026-12-27', 2),
  (7,  8, '2026-12-28', '2026-12-30', 2),
  (8, 13, '2026-12-04', '2026-12-06', 1),
  (9,  1, '2026-12-11', '2026-12-12', 1),
  (10,11, '2026-12-29', '2026-12-31', 2);
