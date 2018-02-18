drop database order_center;
create database order_center;
use order_center;

create table order_operator(
	order_operator_id bigint not null AUTO_INCREMENT,
	display_name varchar(50) not null,
    order_operator_name varchar(50) not null,
	order_operator_password varchar(50) not null, 
	is_active bit not null,
	is_deleted bit not null,
	operator_permission_id int not null,
	last_login_date datetime null,
	date_created datetime not null,
	last_updated datetime not null,
	primary key (order_operator_id)
) AUTO_INCREMENT=1;
insert into order_operator values(null, '管理员','admin','abc123_',1,0,1,null, now(),now());

create table operator_permission(
		operator_permission_id int not null AUTO_INCREMENT,
		allow_to_view_order bit not null,
		allow_to_change_order bit not null,
		allow_to_modify_meal bit not null,
		allow_to_view_sales bit not null,
		allow_to_modify_account bit not null,
		allow_to_modify_payment_method bit not null,
		is_super_admin bit not null,
		order_operator_id bigint not null,
		foreign key(order_operator_id) references order_operator(order_operator_id),
		primary key (operator_permission_id)
) AUTO_INCREMENT=1;
insert into operator_permission values(null, 1, 1, 1, 1, 1, 1, 1, 1);

create table payment_method(
    payment_method_type_id int not null,
	payment_method_name varchar(50) not null,	# cash, alipay, wechat, debit_card(for doctor)
	is_active bit not null,
	date_created datetime not null,
	last_updated datetime not null,
	primary key (payment_method_type_id)
);

insert into payment_method values(1,'现金', 1, now(), now());
insert into payment_method values(2,'支付宝', 1, now(), now());
insert into payment_method values(3,'微信', 1, now(), now());
insert into payment_method values(4,'预付卡', 1, now(), now());



create table lu_device_type(
	device_type_id int not null AUTO_INCREMENT,
	device_type_name varchar(20) not null, #PC, IPAD, iphone Mobile, Android Mobile
	primary key (device_type_id)
) AUTO_INCREMENT=1;
insert into lu_device_type values(null,'Unknown');
insert into lu_device_type values(null,'PC');
insert into lu_device_type values(null,'iPad');
insert into lu_device_type values(null,'iPhone');
insert into lu_device_type values(null,'Android');


create table lu_meal_type(
	meal_type_id int not null,
	meal_type_name varchar(20) not null,
	primary key (meal_type_id)
);
insert into lu_meal_type values(1,'早餐');
insert into lu_meal_type values(2,'中餐');
insert into lu_meal_type values(3,'晚餐');

create table meal(
	meal_id bigint not null AUTO_INCREMENT,
	meal_name varchar(20) not null,
	meal_description varchar(50) not null,
	price float(9,2) not null,
	is_breakfast bit not null,
	is_active bit not null,
	primary key (meal_id)
) AUTO_INCREMENT=1;
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧鱼','好',15, 0, 1);
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧排骨','好',16, 0, 1);
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧肉','好',18, 0, 1);
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧小排','好',12, 0, 1);
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧大排','好',14, 0, 1);
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧牛肉面','好',17, 0, 1);
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧小黄鱼','好',20, 0, 1);
insert into meal(meal_id, meal_name,meal_description,price, is_breakfast, is_active) values(null, '红烧鸡腿','好',19, 0, 1);

create table breakfast(
	breakfast_id bigint not null AUTO_INCREMENT,
	breakfast_name varchar(20) not null,
	breakfast_description varchar(50) not null,
	price float(9,2) not null,
	is_active bit not null,
	image_url varchar(25) null,
	primary key (breakfast_id)
) AUTO_INCREMENT=1;

create table meal_schedule(
	id bigint not null AUTO_INCREMENT ,
	meal_day int not null,#values:1,2,3,4,5,6,7 meanning:Monday-Sunday
	meal_id bigint not null,
	date_created datetime not null,
	last_updated datetime not null,
    meal_type int not null,
	primary key (id)
)AUTO_INCREMENT=1;



create table patient_area(
	id int not null AUTO_INCREMENT,
	area_code varchar(10) not null,
	is_active bit not null,
	primary key (id)
) AUTO_INCREMENT=1;
insert into patient_area values(null,'1A',1);
insert into patient_area values(null,'1B',1);
insert into patient_area values(null,'2A',1);
insert into patient_area values(null,'2B',1);
insert into patient_area values(null,'3A',1);
insert into patient_area values(null,'3B',1);
insert into patient_area values(null,'4A',1);
insert into patient_area values(null,'4B',1);
insert into patient_area values(null,'5A',1);
insert into patient_area values(null,'5B',1);
insert into patient_area values(null,'6A',1);
insert into patient_area values(null,'6B',1);
insert into patient_area values(null,'7A',1);
insert into patient_area values(null,'7B',1);
insert into patient_area values(null,'8A',1);
insert into patient_area values(null,'8B',1);
insert into patient_area values(null,'9A',1);
insert into patient_area values(null,'9B',1);

create table `order`(
	id bigint not null AUTO_INCREMENT,
	bed_number varchar(25) not null,
    patient_area_id int not null, 
	order_operator_id bigint not null,
	device_type_id int not null,
	date_created datetime not null,
	payment_method_id int not null,
	order_affect_day date not null, # 2018-02-09
	receipt_number varchar(8) not null,
    foreign key(patient_area_id) references `patient_area`(id),
    foreign key(order_operator_id) references `order_operator`(order_operator_id),
    foreign key(device_type_id) references `lu_device_type`(device_type_id),
    foreign key(payment_method_id) references `payment_method`(payment_method_type_id),
	primary key (id)
) AUTO_INCREMENT=1;
insert into `order`(id, bed_number, patient_area_id, order_operator_id,device_type_id,date_created,payment_method_id,order_affect_day,receipt_number) 
values(null, '001', 1, 1, 1, now(), 1, '2018-02-16','00000001');
insert into `order`(id, bed_number, patient_area_id, order_operator_id,device_type_id,date_created,payment_method_id,order_affect_day,receipt_number) 
values(null, '002', 2, 1, 1, now(), 1, '2018-02-16','00000002');
insert into `order`(id, bed_number, patient_area_id, order_operator_id,device_type_id,date_created,payment_method_id,order_affect_day,receipt_number) 
values(null, '003', 3, 1, 1, now(), 1, '2018-02-16','00000003');
insert into `order`(id, bed_number, patient_area_id, order_operator_id,device_type_id,date_created,payment_method_id,order_affect_day,receipt_number) 
values(null, '004', 4, 1, 1, now(), 1, '2018-02-16','00000004');

create table meal_order(
	id bigint not null AUTO_INCREMENT,
    order_id bigint not null,
	meal_type int not null,
    meal_id bigint not null,
    foreign key(order_id) references `order`(id),
    foreign key(meal_id) references `meal`(meal_id),
	primary key (id)
) AUTO_INCREMENT=1;

insert into meal_order(id,order_id,meal_type,meal_id) values(null, 1,2,1);
insert into meal_order(id,order_id,meal_type,meal_id) values(null, 2,2,1);
insert into meal_order(id,order_id,meal_type,meal_id) values(null, 3,3,1);
insert into meal_order(id,order_id,meal_type,meal_id) values(null, 4,2,2);
insert into meal_order(id,order_id,meal_type,meal_id) values(null, 4,3,3);
insert into meal_order(id,order_id,meal_type,meal_id) values(null, 2,2,2);
insert into meal_order(id,order_id,meal_type,meal_id) values(null, 3,2,2);
insert into meal_order(id,order_id,meal_type,meal_id) values(null, 2,3,3);




select o.receipt_number,o.order_affect_day, GROUP_CONCAT(mt.meal_type_name,':', m.meal_name) as meal_detail from `order` o
                        inner join meal_order mo
                        on o.id = mo.order_id
                        inner join meal m
                        on mo.meal_id = m.meal_id
                        inner join patient_area pa
                        on o.patient_area_id = pa.id
                        inner join payment_method pm
                        on o.payment_method_id = pm.payment_method_type_id
                        inner join lu_meal_type mt
                        on mo.meal_type = mt.meal_type_id
                        where o.order_affect_day = '2018-02-16'
                        group by o.receipt_number,o.order_affect_day;
                        
                        
                        
                        select o.receipt_number,o.order_affect_day,GROUP_CONCAT(mt.meal_type_name,':', m.meal_name) as meal_details,
                        pa.area_code, o.bed_number, pm.payment_method_name from `order` o
                        inner join meal_order mo
                        on o.id = mo.order_id
                        inner join meal m
                        on mo.meal_id = m.meal_id
                        inner join patient_area pa
                        on o.patient_area_id = pa.id
                        inner join payment_method pm
                        on o.payment_method_id = pm.payment_method_type_id
                        inner join lu_meal_type mt
                        on mo.meal_type = mt.meal_type_id
                        where o.order_affect_day = @order_affect_day
                        group by o.receipt_number,o.order_affect_day,pa.area_code,o.bed_number,pm.payment_method_name


