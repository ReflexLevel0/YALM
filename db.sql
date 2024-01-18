DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

CREATE TABLE cpu
(
    serverid     integer PRIMARY KEY,
    name         varchar(256),
    architecture varchar(256),
    cores        integer,
    threads      integer,
    frequencyMHz integer
);

CREATE TABLE cpuLog
(
    serverid      integer   NOT NULL,
    date          timestamp NOT NULL,
    interval      integer   NOT NULL,
    usage         numeric,
    numberoftasks integer,
    PRIMARY KEY (serverid, date)
);

CREATE TABLE programLog
(
    serverid                    integer   NOT NULL,
    date                        timestamp NOT NULL,
    name                        varchar(255) NOT NULL,
    interval                    integer   NOT NULL,
    cpuutilizationpercentage    numeric,
    memoryutilizationpercentage numeric,
    PRIMARY KEY (serverid, date, name)
);

CREATE TABLE memoryLog
(
    serverid       integer   NOT NULL,
    date           timestamp NOT NULL,
    interval       bigint    NOT NULL,
    totalKb        bigint, --memory size (excluding swap size) 
    freeKb         bigint, --memory that isn't used or cached
    usedKb         bigint, --used memory
    swapTotalKb    bigint, --swap memory size
    swapFreeKb     bigint, --swap memory that isn't used or cached
    swapUsedKb     bigint, --used swap memory
    availableKb    bigint, --memory that isn't used (including unused swap memory)
    cachedKb       bigint, --cached memory
    PRIMARY KEY (serverid, date)
);

CREATE TABLE disk
(
    id       SERIAL,
    serverid integer NOT NULL,
    label    VARCHAR(256),
    PRIMARY KEY (id),
    UNIQUE (serverid, label)
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
    FOREIGN KEY (diskId) REFERENCES disk ON DELETE CASCADE
);

CREATE TABLE partitionLog
(
    diskId     SERIAL      NOT NULL,
    uuid       varchar(64) NOT NULL,
    date       timestamp   NOT NULL,
    interval   integer     NOT NULL,
    bytestotal bigint,
    usage      decimal(3, 2),
    PRIMARY KEY (diskId, date, uuid),
    FOREIGN KEY (diskId, uuid) REFERENCES partition ON DELETE CASCADE
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