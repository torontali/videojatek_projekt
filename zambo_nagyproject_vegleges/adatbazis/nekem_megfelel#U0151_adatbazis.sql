-- --------------------------------------------------------
-- Gazdagép:                     127.0.0.1
-- Szerver verzió:               9.5.0 - MySQL Community Server - GPL
-- Szerver OS:                   Linux
-- HeidiSQL Verzió:              12.12.0.7122
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Adatbázis struktúra mentése a prjektanp_dbvel.
CREATE DATABASE IF NOT EXISTS `prjektanp_dbvel` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `prjektanp_dbvel`;

-- Struktúra mentése tábla prjektanp_dbvel. Games
CREATE TABLE IF NOT EXISTS `Games` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `Platform` varchar(50) DEFAULT NULL,
  `ReleaseDate` varchar(30) DEFAULT NULL,
  `Company` varchar(100) DEFAULT NULL,
  `CoverImage` varchar(255) DEFAULT NULL,
  `Votes` int NOT NULL DEFAULT '0',
  `TypeId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_games_types` (`TypeId`),
  CONSTRAINT `fk_games_types` FOREIGN KEY (`TypeId`) REFERENCES `Types` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tábla adatainak mentése prjektanp_dbvel.Games: ~4 rows (hozzávetőleg)
INSERT INTO `Games` (`Id`, `Name`, `Platform`, `ReleaseDate`, `Company`, `CoverImage`, `Votes`, `TypeId`) VALUES
	(1, 'Cyberpunk 2077', 'PC', '2020. 12. 10.', 'CD Projekt Red', 'https://via.placeholder.com/150', 142, 2),
	(2, 'The Witcher 3: Wild Hunt', 'XBOX', '2015. 05. 19.', 'CD Projekt Red', 'https://via.placeholder.com/150', 389, 2),
	(3, 'Grand Theft Auto V', 'PlayStation', '2013. 09. 17.', 'Rockstar Games', 'https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg', 512, 1),
	(4, 'Minecraft', 'PC', '2011. 11. 18.', 'Mojang Studios', 'https://via.placeholder.com/150', 780, 4);

-- Struktúra mentése tábla prjektanp_dbvel. Players
CREATE TABLE IF NOT EXISTS `Players` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Level` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Username` (`Username`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tábla adatainak mentése prjektanp_dbvel.Players: ~0 rows (hozzávetőleg)
INSERT INTO `Players` (`Id`, `Username`, `Email`, `Level`) VALUES
	(1, 'gamer_pro99', 'pro99@example.com', 5),
	(2, 'anna_gamer', 'anna@example.com', 3),
	(3, 'Kalandor92', 'kalandor@example.com', 10);

-- Struktúra mentése tábla prjektanp_dbvel. Reviews
CREATE TABLE IF NOT EXISTS `Reviews` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PlayerId` int NOT NULL,
  `GameId` int NOT NULL,
  `Points` int NOT NULL,
  `Comment` varchar(500) DEFAULT NULL,
  `ReviewReleaseDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `fk_reviews_players` (`PlayerId`),
  KEY `fk_reviews_games` (`GameId`),
  CONSTRAINT `fk_reviews_games` FOREIGN KEY (`GameId`) REFERENCES `Games` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_reviews_players` FOREIGN KEY (`PlayerId`) REFERENCES `Players` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tábla adatainak mentése prjektanp_dbvel.Reviews: ~4 rows (hozzávetőleg)
INSERT INTO `Reviews` (`Id`, `PlayerId`, `GameId`, `Points`, `Comment`, `ReviewReleaseDate`) VALUES
	(1, 1, 1, 5, 'Fantasztikus hangulat és történet!', '2026-10-27 16:56:33'),
	(2, 2, 1, 4, 'Nagyon jó, bár volt néhány bug.', '2026-10-27 16:56:33'),
	(3, 3, 2, 5, 'Minden idők egyik legjobb RPG-je!', '2026-10-27 16:56:33'),
	(4, 1, 3, 4, 'Örök klasszikus, imádom a nyitott világot.', '2026-10-27 16:56:33');

-- Struktúra mentése tábla prjektanp_dbvel. Types
CREATE TABLE IF NOT EXISTS `Types` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Name` (`Name`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tábla adatainak mentése prjektanp_dbvel.Types: ~4 rows (hozzávetőleg)
INSERT INTO `Types` (`Id`, `Name`) VALUES
	(1, 'Akció'),
	(2, 'RPG'),
	(3, 'Stratégia'),
	(4, 'Sandbox');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
