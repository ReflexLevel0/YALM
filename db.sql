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
    FOREIGN KEY (serverId) REFERENCES cpu (serverId)
);

CREATE TABLE programLog
(
    serverid                    integer      NOT NULL,
    date                        timestamp    NOT NULL,
    name                        varchar(255) NOT NULL,
    interval                    integer      NOT NULL,
    cpuutilizationpercentage    numeric,
    memoryutilizationpercentage numeric,
    PRIMARY KEY (serverid, date, name)
);

CREATE TABLE memoryLog
(
    serverid    integer   NOT NULL,
    date        timestamp NOT NULL,
    interval    bigint    NOT NULL,
    totalKb     bigint, --memory size (excluding swap size) 
    freeKb      bigint, --memory that isn't used or cached
    usedKb      bigint, --used memory
    swapTotalKb bigint, --swap memory size
    swapFreeKb  bigint, --swap memory that isn't used or cached
    swapUsedKb  bigint, --used swap memory
    availableKb bigint, --memory that isn't used (including unused swap memory)
    cachedKb    bigint, --cached memory
    PRIMARY KEY (serverid, date)
);

CREATE TABLE disk
(
    uuid       VARCHAR(256),     --unique ID of the disk
    serverid   INTEGER NOT NULL, --server ID to which the disk belongs
    type       VARCHAR(32),      --type of the disk (ex: gpt)
    serial     VARCHAR(256),     --globally unique (generally unique, but not guaranteed) disk ID
    path       VARCHAR(256),     --disk path (ex: /dev/sda) 
    vendor     VARCHAR(256),     --manufacturer of the disk (ex: ATA)
    model      VARCHAR(256),     --disk name (ex: Samsung EVO 970)
    bytesTotal BIGINT,           --size of the disk in bytes
    PRIMARY KEY (uuid, serverid)
);

CREATE TABLE partition
(
    diskUuid          varchar(256) NOT NULL,
    uuid              varchar(64),
    serverId          integer,
    filesystemName    varchar(64),
    filesystemVersion varchar(64),
    label             varchar(256),
    mountpath         varchar(1024),
    UNIQUE (serverId, uuid),
    PRIMARY KEY (serverId, uuid),
    FOREIGN KEY (diskUuid, serverId) REFERENCES disk (uuid, serverId) ON DELETE CASCADE
);

CREATE TABLE partitionLog
(
    serverId      integer     NOT NULL,
    partitionUuid varchar(64) NOT NULL,
    date          timestamp   NOT NULL,
    interval      integer     NOT NULL,
    bytestotal    bigint,
    usage         decimal(3, 2),
    PRIMARY KEY (serverId, partitionUuid, date),
    FOREIGN KEY (serverId, partitionUuid) REFERENCES partition (serverId, uuid) ON DELETE CASCADE
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

CREATE TABLE server(
	serverid integer NOT NULL PRIMARY KEY
);

CREATE TABLE serverlog(
	serverid integer NOT NULL,
	date timestamp NOT NULL,
	interval integer NOT NULL,
	PRIMARY KEY (serverid, date),
	FOREIGN KEY (serverid) REFERENCES server(serverid)
);

CREATE VIEW serverStatus AS
SELECT serverid, (SELECT CASE WHEN COUNT(*) > 0 THEN 'online' ELSE 'offline' END
                  FROM serverlog
                  WHERE
                      serverlog.serverid = server.serverid AND
                      EXTRACT(EPOCH FROM timezone('utc', now())-serverlog.date) < (serverlog.interval * 60)) as status
FROM server;

CREATE TABLE alert
(
    serverId integer NOT NULL,
    date timestamp NOT NULL,
	severity int NOT NULL,
    text varchar(256) NOT NULL,
    primary key (serverId, date, text)
);

CREATE OR REPLACE FUNCTION before_alert_insert_func()
    RETURNS TRIGGER
    LANGUAGE PLPGSQL
AS
$$
DECLARE
    mins INT;
BEGIN
    SELECT EXTRACT(EPOCH FROM NEW.date::timestamp-date::timestamp)/60 FROM alert WHERE NEW.serverId = serverId AND NEW.text LIKE text ORDER BY date desc INTO mins;
    IF mins < 60 THEN
        RAISE EXCEPTION 'Same alert already raised less then an hour ago (% minutes ago)', mins;
    END IF;
    RETURN NEW;
END
$$;
    
CREATE TRIGGER before_alert_insert
    BEFORE INSERT
    ON alert
    FOR EACH ROW
    EXECUTE FUNCTION before_alert_insert_func();

-- testing alerts
DELETE FROM alert WHERE 1=1;
INSERT INTO alert(serverId, date, severity, text) VALUES(0, '2024-05-30 00:00:00', 3, 'Cpu usage above 90%');
INSERT INTO alert(serverId, date, severity, text) VALUES(0, '2024-05-30 01:30:00', 3, 'Cpu usage above 90%');
INSERT INTO alert(serverId, date, severity, text) VALUES(1, '2024-05-30 02:00:00', 3, 'Cpu usage above 90%');
INSERT INTO alert(serverId, date, severity, text) VALUES(0, '2024-05-30 02:15:00', 2, 'Memory usage above 50%');
-- INSERT INTO alert(serverId, date, text) VALUES(0, '2024-05-30 02:15:00', '[WARNING] Cpu usage above 90%');
INSERT INTO alert(serverId, date, severity, text) VALUES(0, '2024-06-02 12:33:33', 1, 'This is an info message');

-- testing server status
INSERT INTO server VALUES(0),(1);
INSERT INTO serverlog(serverid, date, interval) VALUES(0, now()::timestamp-interval '660 seconds', 5),
                                                      (0, now()::timestamp-interval '360 seconds', 5),
                                                      (0, now()::timestamp-interval '60 seconds', 5);
INSERT INTO serverlog(serverid, date, interval) VALUES(1, now()::timestamp-interval '630 seconds', 5),
                                                      (1, now()::timestamp-interval '330 seconds', 5);
SELECT * FROM serverstatus;