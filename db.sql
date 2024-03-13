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
    PRIMARY KEY (serverid, date),
    FOREIGN KEY (serverId) REFERENCES cpu(serverId)
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
    uuid        VARCHAR(256),       --unique ID of the disk
    serverid    INTEGER NOT NULL,   --server ID to which the disk belongs
    type        VARCHAR(32),        --type of the disk (ex: gpt)
    serial      VARCHAR(256),       --globally unique (generally unique, but not guaranteed) disk ID
    path        VARCHAR(256),       --disk path (ex: /dev/sda) 
    vendor      VARCHAR(256),       --manufacturer of the disk (ex: ATA)
    model       VARCHAR(256),       --disk name (ex: Samsung EVO 970)
    bytesTotal  BIGINT,             --size of the disk in bytes
    PRIMARY KEY (uuid, serverid)
);

CREATE TABLE partition
(
    diskUuid          varchar(256),
    uuid              varchar(64) NOT NULL,
    serverId          integer NOT NULL,
    filesystemName    varchar(64),
    filesystemVersion varchar(64),
    label             varchar(256),
    mountpath         varchar(1024),
    UNIQUE(serverId, uuid),
    PRIMARY KEY (serverId, uuid),
    FOREIGN KEY (diskUuid, serverId) REFERENCES disk(uuid, serverId) ON DELETE CASCADE
);

CREATE TABLE partitionLog
(
    serverId        integer NOT NULL,
    partitionUuid   varchar(64) NOT NULL,
    date            timestamp   NOT NULL,
    interval        integer     NOT NULL,
    bytestotal      bigint,
    usage           decimal(3, 2),
    PRIMARY KEY (serverId, partitionUuid, date),
    FOREIGN KEY (serverId, partitionUuid) REFERENCES partition(serverId, uuid) ON DELETE CASCADE
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