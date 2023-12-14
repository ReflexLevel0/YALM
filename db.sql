DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
CREATE TABLE cpu
(
    serverid      integer   NOT NULL,
    date          timestamp NOT NULL,
    interval      integer   NOT NULL,
    usage         numeric,
    numberoftasks integer,
    PRIMARY KEY (serverid, date)
);

CREATE TABLE program
(
    serverid                    integer   NOT NULL,
    date                        timestamp NOT NULL,
    interval                    integer   NOT NULL,
    cpuutilizationpercentage    numeric,
    memoryutilizationpercentage numeric
);

CREATE TABLE memory
(
    serverid integer   NOT NULL,
    date     timestamp NOT NULL,
    interval integer   NOT NULL,
    mbused   integer,
    mbtotal  integer,
    PRIMARY KEY (serverid, date)
);

CREATE TABLE disk
(
    id    SERIAL,
    serverid integer NOT NULL,
    label VARCHAR(256),
    PRIMARY KEY(id)
);

CREATE TABLE partition
(
    diskId            SERIAL      NOT NULL,
    uuid              varchar(64) NOT NULL,
    filesystemName    varchar(64),
    filesystemVersion varchar(64),
    label             varchar(256),
    mountpath         varchar(1024),
    PRIMARY KEY (diskId, uuid),
    FOREIGN KEY (diskId) REFERENCES disk
);

CREATE TABLE partitionLog
(
    serverId   integer     NOT NULL,
    uuid       varchar(64) NOT NULL,
    date       timestamp   NOT NULL,
    interval   integer     NOT NULL,
    bytestotal bigint,
    usage      decimal(3, 2),
    PRIMARY KEY (serverid, date, uuid),
    FOREIGN KEY (serverId, uuid) REFERENCES partition
);

CREATE TABLE servicename
(
    serviceid integer NOT NULL PRIMARY KEY,
    name      varchar(64)
);

CREATE TABLE servicestatus
(
    statusid integer NOT NULL PRIMARY KEY,
    name     varchar(64)
);

CREATE TABLE service
(
    serverid          integer   NOT NULL,
    serviceid         integer   NOT NULL REFERENCES servicename,
    date              timestamp NOT NULL,
    interval          integer   NOT NULL,
    ramusagemegabytes integer,
    statusid          integer REFERENCES servicestatus,
    tasks             integer,
    cpuseconds        numeric,
    PRIMARY KEY (serverid, serviceid, date)
);

CREATE TABLE servicelog
(
    serverid    integer       NOT NULL,
    serviceid   integer       NOT NULL REFERENCES servicename,
    interval    integer       NOT NULL,
    date        timestamp     NOT NULL,
    messagetext varchar(1024) NOT NULL,
    PRIMARY KEY (serverid, serviceid, date, messagetext)
);