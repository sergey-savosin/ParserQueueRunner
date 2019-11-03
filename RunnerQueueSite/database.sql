-- phpMyAdmin SQL Dump
-- version 4.0.10.20
-- https://www.phpmyadmin.net
--
-- Host: 10.0.0.147:3306
-- Generation Time: Nov 03, 2019 at 10:29 PM
-- Server version: 10.1.40-MariaDB
-- PHP Version: 5.3.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `vprofy_runnerqueue`
--

-- --------------------------------------------------------

--
-- Table structure for table `queuestatus`
--

CREATE TABLE IF NOT EXISTS `queuestatus` (
  `queuestatusid` int(11) NOT NULL,
  `statusname` varchar(50) NOT NULL,
  PRIMARY KEY (`queuestatusid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `queuestatus`
--

INSERT INTO `queuestatus` (`queuestatusid`, `statusname`) VALUES
(1, 'NewRecord'),
(2, 'Processing'),
(3, 'Done'),
(4, 'DoneWithError');

-- --------------------------------------------------------

--
-- Table structure for table `runnerqueue`
--

CREATE TABLE IF NOT EXISTS `runnerqueue` (
  `RunnerQueueId` int(11) NOT NULL AUTO_INCREMENT,
  `CommandText` varchar(500) NOT NULL,
  `ErrorText` varchar(2000) DEFAULT NULL,
  `CreatedTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedTime` datetime DEFAULT NULL,
  `QueueStatusId` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`RunnerQueueId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=81 ;


/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
