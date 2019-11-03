-- phpMyAdmin SQL Dump
-- version 4.0.10.20
-- https://www.phpmyadmin.net
--
-- Host: 10.0.0.147:3306
-- Generation Time: Nov 03, 2019 at 10:28 PM
-- Server version: 10.1.40-MariaDB
-- PHP Version: 5.3.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `vprofy_parserqueue`
--

-- --------------------------------------------------------

--
-- Table structure for table `parserqueue`
--

CREATE TABLE IF NOT EXISTS `parserqueue` (
  `ParserQueueId` int(11) NOT NULL AUTO_INCREMENT,
  `ClientEmail` varchar(250) NOT NULL,
  `ClientDocNum` varchar(250) NOT NULL,
  `CreatedTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedTime` datetime DEFAULT NULL,
  `QueueStatusId` int(11) NOT NULL DEFAULT '1',
  `ErrorText` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`ParserQueueId`),
  KEY `QueueStatusId` (`QueueStatusId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=292 ;


-- --------------------------------------------------------

--
-- Table structure for table `queuestatus`
--

CREATE TABLE IF NOT EXISTS `queuestatus` (
  `queuestatusid` int(11) NOT NULL AUTO_INCREMENT,
  `statusname` varchar(250) NOT NULL,
  PRIMARY KEY (`queuestatusid`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=5 ;

--
-- Dumping data for table `queuestatus`
--

INSERT INTO `queuestatus` (`queuestatusid`, `statusname`) VALUES
(1, 'New'),
(2, 'Processing'),
(3, 'EmailSent'),
(4, 'Error');

--
-- Constraints for dumped tables
--

--
-- Constraints for table `parserqueue`
--
ALTER TABLE `parserqueue`
  ADD CONSTRAINT `FK_ParserQueue_QueueStatus` FOREIGN KEY (`QueueStatusId`) REFERENCES `queuestatus` (`queuestatusid`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
