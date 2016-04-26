create table accounting.tb_report
(
	cd_report	char(4)			not	null	
	,ds_report	varchar(100)	not	null
	,constraint pk_tb_report	primary key (cd_report)
)

insert into accounting.tb_report 
select 'BLST'	cd_report	,'Balance Sheet'	ds_report
union
select 'INCS'	cd_report	,'Income Statement'	ds_report


drop table accounting.tb_account_category 
create table accounting.tb_account_category 
(
	cd_account_category		char(4)	not	null
	,ds_account_category	varchar(50)	not	null
	,cd_report	char(4)			not	null	
	,constraint pk_tb_account_category primary key (cd_account_category)
	,constraint fk_tb_account_category_tb_report foreign key (cd_report)
		references accounting.tb_report (cd_report)	
)
go

insert into accounting.tb_account_category 
select 'ASST'	cd_account_category	,'Assets'	ds_account_category, 'BLST'	cd_report
union
select 'LIAB'	cd_account_category	,'Liabilities'	ds_account_category, 'BLST'	cd_report
union
select 'OWEQ'	cd_account_category	,'Owners Equity'	ds_account_category, 'BLST'	cd_report
union
select 'REVE'	cd_account_category	,'Revenues'	ds_account_category, 'INCS'	cd_report
union
select 'EXPE'	cd_account_category	,'Expenses'	ds_account_category, 'INCS'	cd_report


drop table accounting.tb_account_chart
create table accounting.tb_account_chart
(
	id_account			int	identity(1,1)	not	null
	,id_parent_account	int					null
	,nr_account			varchar(20)			not null
	,ds_account			varchar(50)			not	null
	,cd_account_category	char(4)			not	null
	,vl_balance				money			not	null
	,constraint	pk_tb_account_chart primary key (id_account)
	,constraint iu_tb_account_chart_nr_account unique (nr_account)
	,constraint fk_tb_account_chart_tb_account_chart foreign key (id_parent_account)
		references accounting.tb_account_chart (id_account)	
	,constraint fk_tb_account_chart_tb_account_category foreign key (cd_account_category)
		references accounting.tb_account_category (cd_account_category)	
	
)
go

drop table accounting.tb_general_journal
create table accounting.tb_general_journal
(
	id_entry		int	identity(1,1)	not	null
	,dt_entry		datetime			not	null
	,ds_entry		varchar(200)		not	null
	,vl_debit		money				null
	,vl_credit		money				null
	,id_account		int					not null
	,constraint pk_tb_general_journal	primary key (id_entry)
	,constraint fk_tb_general_journal_tb_account_chart foreign key (id_account)
		references accounting.tb_account_chart (id_account)
)
go

