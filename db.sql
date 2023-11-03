DROP TABLE IF EXISTS cpu CASCADE ;
DROP TABLE IF EXISTS program CASCADE ;
DROP TABLE IF EXISTS memory CASCADE ;
DROP TABLE IF EXISTS service CASCADE ;
DROP TABLE IF EXISTS servicelog CASCADE ;
DROP TABLE IF EXISTS servicename CASCADE ;
DROP TABLE IF EXISTS servicestatus CASCADE ;
DROP TABLE IF EXISTS storage CASCADE ;
CREATE TABLE cpu
(
	serverid      integer   NOT NULL,
	date          timestamp NOT NULL,
	interval      integer,
	usage         numeric,
	numberoftasks integer,
	PRIMARY KEY (serverid, date)
);

CREATE TABLE program
(
	serverid                    integer,
	date                        timestamp,
	interval                    integer,
	cpuutilizationpercentage    numeric,
	memoryutilizationpercentage numeric
);

CREATE TABLE memory
(
	serverid integer   NOT NULL,
	date     timestamp NOT NULL,
	interval integer,
	mbused   integer,
	mbtotal  integer,
	PRIMARY KEY (serverid, date)
);

CREATE TABLE storage
(
	serverid   integer      NOT NULL,
	date       timestamp    NOT NULL,
	interval   integer,
	filesystem varchar(256) NOT NULL,
	mountpath  varchar(512),
	bytestotal bigint,
	usedbytes  bigint,
	PRIMARY KEY (serverid, date, filesystem)
);

CREATE TABLE servicename
(
	serviceid integer NOT NULL
		PRIMARY KEY,
	name      varchar(64)
);

CREATE TABLE servicestatus
(
	statusid integer NOT NULL
		PRIMARY KEY,
	name     varchar(64)
);

CREATE TABLE service
(
	serverid          integer   NOT NULL,
	serviceid         integer   NOT NULL
		REFERENCES servicename,
	date              timestamp NOT NULL,
	interval          integer,
	ramusagemegabytes integer,
	statusid          integer
		REFERENCES servicestatus,
	tasks             integer,
	cpuseconds        numeric,
	PRIMARY KEY (serverid, serviceid, date)
);

CREATE TABLE servicelog
(
	serverid    integer       NOT NULL,
	serviceid   integer       NOT NULL
		REFERENCES servicename,
	interval    integer,
	date        timestamp     NOT NULL,
	messagetext varchar(1024) NOT NULL,
	PRIMARY KEY (serverid, serviceid, date, messagetext)
);

SELECT * FROM cpu;