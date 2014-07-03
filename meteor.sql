/*
Navicat MySQL Data Transfer

Source Server         : Localhost
Source Server Version : 50612
Source Host           : localhost:3306
Source Database       : meteor

Target Server Type    : MYSQL
Target Server Version : 50612
File Encoding         : 65001

Date: 2014-04-16 16:10:14
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `accounts`
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `OldAccountId` int(11) NOT NULL,
  `Username` varchar(32) NOT NULL,
  `Password` varchar(32) NOT NULL,
  `LastIp` varchar(255) NOT NULL,
  `Authority` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of accounts
-- ----------------------------
INSERT INTO accounts VALUES ('1', '-1', 'shyro', '098f6bcd4621d373cade4e832627b4f6', '79212918711715106117182209481722053821122162043217182', '7');

-- ----------------------------
-- Table structure for `characters`
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `AccountId` int(11) NOT NULL,
  `OldAccountId` int(11) NOT NULL DEFAULT '-1',
  `Name` varchar(35) NOT NULL,
  `Slot` int(11) NOT NULL,
  `Level` int(11) NOT NULL,
  `Job` int(11) NOT NULL,
  `Exp` int(11) NOT NULL,
  `Gold` int(11) NOT NULL,
  `Gender` int(2) NOT NULL,
  `MapId` int(11) NOT NULL,
  `PosX` float(11,0) NOT NULL,
  `PosY` float(11,0) NOT NULL,
  `PosZ` float(11,0) NOT NULL,
  `Strength` int(11) NOT NULL,
  `Stamina` int(11) NOT NULL,
  `Dexterity` int(11) NOT NULL,
  `Inteligence` int(11) NOT NULL,
  `Spirit` int(11) NOT NULL,
  `Hp` int(11) NOT NULL,
  `Mp` int(11) NOT NULL,
  `HairMesh` int(11) NOT NULL,
  `HairColor` int(11) NOT NULL,
  `HeadMesh` int(11) NOT NULL,
  `DeletedDate` varchar(35) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=56 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of characters
-- ----------------------------
INSERT INTO characters VALUES ('55', '1', '-1', 'Shyro', '0', '1', '5', '0', '0', '0', '1', '618', '230', '611', '15', '15', '15', '15', '15', '200', '200', '0', '-1020247', '0', '4/14/2014 4:12:35 PM');

-- ----------------------------
-- Table structure for `closet`
-- ----------------------------
DROP TABLE IF EXISTS `closet`;
CREATE TABLE `closet` (
  `Id` int(11) NOT NULL,
  `CharacterId` int(11) NOT NULL,
  `Slot` int(11) NOT NULL,
  `Level` int(11) NOT NULL,
  `Equiped` int(11) NOT NULL,
  `Date` varchar(255) NOT NULL,
  PRIMARY KEY (`CharacterId`,`Slot`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of closet
-- ----------------------------

-- ----------------------------
-- Table structure for `items`
-- ----------------------------
DROP TABLE IF EXISTS `items`;
CREATE TABLE `items` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CharacterId` int(11) NOT NULL,
  `CreatorId` int(11) NOT NULL DEFAULT '-1',
  `Slot` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL,
  `Equiped` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of items
-- ----------------------------
