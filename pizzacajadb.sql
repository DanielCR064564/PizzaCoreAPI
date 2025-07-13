CREATE DATABASE  IF NOT EXISTS `pizzacajadb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `pizzacajadb`;
-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: localhost    Database: pizzacajadb
-- ------------------------------------------------------
-- Server version	9.2.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `auditoria`
--

DROP TABLE IF EXISTS `auditoria`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auditoria` (
  `idauditoria` int NOT NULL AUTO_INCREMENT,
  `empleadoId` int DEFAULT NULL,
  `accion` varchar(100) NOT NULL,
  `tabla` varchar(50) NOT NULL,
  `registroId` int DEFAULT NULL,
  `datosAnteriores` json DEFAULT NULL,
  `datosNuevos` json DEFAULT NULL,
  `fechaAccion` datetime DEFAULT CURRENT_TIMESTAMP,
  `ipAddress` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`idauditoria`),
  KEY `idx_auditoria_fecha` (`fechaAccion`),
  KEY `idx_auditoria_tabla` (`tabla`),
  KEY `idx_auditoria_empleado` (`empleadoId`),
  CONSTRAINT `auditoria_ibfk_1` FOREIGN KEY (`empleadoId`) REFERENCES `empleado` (`idempleado`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Registro de auditoría del sistema';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `auditoria`
--

LOCK TABLES `auditoria` WRITE;
/*!40000 ALTER TABLE `auditoria` DISABLE KEYS */;
/*!40000 ALTER TABLE `auditoria` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `caja_registradora`
--

DROP TABLE IF EXISTS `caja_registradora`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `caja_registradora` (
  `idcaja` int NOT NULL AUTO_INCREMENT,
  `empleadoId` int NOT NULL,
  `fechaApertura` datetime DEFAULT CURRENT_TIMESTAMP,
  `fechaCierre` datetime DEFAULT NULL,
  `montoInicial` decimal(12,2) NOT NULL DEFAULT '0.00',
  `montoFinal` decimal(12,2) DEFAULT '0.00',
  `totalVentas` decimal(12,2) DEFAULT '0.00',
  `totalEfectivo` decimal(12,2) DEFAULT '0.00',
  `totalTarjetas` decimal(12,2) DEFAULT '0.00',
  `diferencia` decimal(12,2) DEFAULT '0.00',
  `notas` text,
  `estado` enum('Abierta','Cerrada') DEFAULT 'Abierta',
  PRIMARY KEY (`idcaja`),
  KEY `idx_caja_fecha` (`fechaApertura`),
  KEY `idx_caja_empleado` (`empleadoId`),
  KEY `idx_caja_estado` (`estado`),
  CONSTRAINT `caja_registradora_ibfk_1` FOREIGN KEY (`empleadoId`) REFERENCES `empleado` (`idempleado`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Control de caja registradora';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `caja_registradora`
--

LOCK TABLES `caja_registradora` WRITE;
/*!40000 ALTER TABLE `caja_registradora` DISABLE KEYS */;
/*!40000 ALTER TABLE `caja_registradora` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cliente`
--

DROP TABLE IF EXISTS `cliente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cliente` (
  `idCliente` int NOT NULL AUTO_INCREMENT,
  `nombres` varchar(100) NOT NULL,
  `apPaterno` varchar(100) NOT NULL,
  `apMaterno` varchar(100) NOT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `direccion` text,
  `ciudad` varchar(100) DEFAULT NULL,
  `provincia` varchar(100) DEFAULT NULL,
  `codigoPostal` varchar(10) DEFAULT NULL,
  `rnc` varchar(20) DEFAULT NULL COMMENT 'RNC o Cédula del cliente',
  `esFrecuente` tinyint(1) DEFAULT '0',
  `notas` text,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  `totalCompras` decimal(12,2) DEFAULT '0.00',
  `numeroOrdenes` int DEFAULT '0',
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`idCliente`),
  KEY `idx_cliente_nombres` (`nombres`),
  KEY `idx_cliente_telefono` (`telefono`),
  KEY `idx_cliente_email` (`email`),
  KEY `idx_cliente_frecuente` (`esFrecuente`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Tabla de clientes del sistema POS';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cliente`
--

LOCK TABLES `cliente` WRITE;
/*!40000 ALTER TABLE `cliente` DISABLE KEYS */;
INSERT INTO `cliente` VALUES (1,'Cliente','General','Modificacion','cliente@general.com','000-000-0000','Dirección no especificada','Santo Domingo Este','Santo Domingo',NULL,NULL,0,NULL,'2025-07-13 02:11:37',0.00,0,1);
/*!40000 ALTER TABLE `cliente` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `configuracion`
--

DROP TABLE IF EXISTS `configuracion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `configuracion` (
  `idconfig` int NOT NULL AUTO_INCREMENT,
  `clave` varchar(100) NOT NULL,
  `valor` text NOT NULL,
  `descripcion` text,
  `categoria` varchar(50) DEFAULT 'General',
  `fechaActualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`idconfig`),
  UNIQUE KEY `clave` (`clave`),
  KEY `idx_config_categoria` (`categoria`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Configuraciones del sistema';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `configuracion`
--

LOCK TABLES `configuracion` WRITE;
/*!40000 ALTER TABLE `configuracion` DISABLE KEYS */;
INSERT INTO `configuracion` VALUES (1,'empresa_nombre','Pizza Caja','Nombre de la empresa','General','2025-07-13 02:11:37'),(2,'empresa_direccion','Santo Domingo Este, Rep. Dom.','Dirección de la empresa','General','2025-07-13 02:11:37'),(3,'empresa_telefono','809-123-4567','Teléfono de la empresa','General','2025-07-13 02:11:37'),(4,'empresa_rnc','123456789','RNC de la empresa','General','2025-07-13 02:11:37'),(5,'iva_porcentaje','18','Porcentaje de IVA (18%)','Impuestos','2025-07-13 02:11:37'),(6,'delivery_costo','150.00','Costo de delivery en RD$','Ventas','2025-07-13 02:11:37'),(7,'api_url','https://localhost:5201/api','URL de la API','Integracion','2025-07-13 02:11:37'),(8,'auto_sync_api','false','Sincronización automática con API','Integracion','2025-07-13 02:11:37');
/*!40000 ALTER TABLE `configuracion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `configuracion_sync`
--

DROP TABLE IF EXISTS `configuracion_sync`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `configuracion_sync` (
  `clave` varchar(50) NOT NULL,
  `valor` text NOT NULL,
  `tipo` enum('string','int','bool','datetime') DEFAULT 'string',
  `categoria` varchar(50) DEFAULT 'general',
  `descripcion` text,
  `fecha_actualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`clave`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `configuracion_sync`
--

LOCK TABLES `configuracion_sync` WRITE;
/*!40000 ALTER TABLE `configuracion_sync` DISABLE KEYS */;
INSERT INTO `configuracion_sync` VALUES ('api_base_url','https://localhost:5201/api','string','api','URL base de la API','2025-07-13 03:08:54'),('max_reintentos','3','int','sync','Máximo número de reintentos para operaciones fallidas','2025-07-13 03:08:54'),('modo_offline','false','bool','operacion','Modo offline forzado','2025-07-13 03:08:54'),('sync_interval_productos','300','int','sync','Intervalo de sincronización de productos (segundos)','2025-07-13 03:08:54'),('sync_interval_ventas','60','int','sync','Intervalo de sincronización de ventas (segundos)','2025-07-13 03:08:54'),('terminal_id','3c8a2b90-5fb8-11f0-a049-34298f730067','string','identificacion','ID único del terminal POS','2025-07-13 03:08:54'),('timeout_api','30','int','api','Timeout para llamadas API (segundos)','2025-07-13 03:08:54'),('ultima_sync_productos','2025-07-13 12:29:39','datetime','sync','Última sincronización de productos','2025-07-13 12:29:39');
/*!40000 ALTER TABLE `configuracion_sync` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalle`
--

DROP TABLE IF EXISTS `detalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalle` (
  `iddetalle` int NOT NULL AUTO_INCREMENT,
  `fkfactura` int NOT NULL,
  `fkproducto` int NOT NULL,
  `cantidad` int NOT NULL DEFAULT '1',
  `precioventa` decimal(10,2) NOT NULL,
  `subtotal` decimal(12,2) NOT NULL,
  `notas` text COMMENT 'Ingredientes extra o notas especiales',
  PRIMARY KEY (`iddetalle`),
  KEY `idx_detalle_factura` (`fkfactura`),
  KEY `idx_detalle_producto` (`fkproducto`),
  CONSTRAINT `detalle_ibfk_1` FOREIGN KEY (`fkfactura`) REFERENCES `factura` (`idfactura`) ON DELETE CASCADE,
  CONSTRAINT `detalle_ibfk_2` FOREIGN KEY (`fkproducto`) REFERENCES `producto` (`idproducto`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Detalles de productos en cada factura';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalle`
--

LOCK TABLES `detalle` WRITE;
/*!40000 ALTER TABLE `detalle` DISABLE KEYS */;
INSERT INTO `detalle` VALUES (1,1,1,2,450.00,0.00,NULL),(2,1,7,1,60.00,0.00,NULL),(3,1,1,2,450.00,0.00,NULL),(4,1,7,1,60.00,0.00,NULL),(5,4,1,1,450.00,450.00,''),(6,4,8,2,60.00,120.00,''),(7,5,1,1,450.00,450.00,''),(8,5,8,2,60.00,120.00,''),(9,6,1,1,450.00,450.00,''),(10,7,7,1,60.00,60.00,''),(11,8,18,10,1.00,10.00,''),(12,9,5,1,580.00,580.00,'');
/*!40000 ALTER TABLE `detalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalle_venta`
--

DROP TABLE IF EXISTS `detalle_venta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalle_venta` (
  `iddetalle` int NOT NULL AUTO_INCREMENT,
  `ventaId` int NOT NULL,
  `productoId` int NOT NULL,
  `cantidad` int NOT NULL DEFAULT '1',
  `precioUnitario` decimal(10,2) NOT NULL,
  `subtotal` decimal(12,2) NOT NULL,
  `notas` text,
  `ingredientesExtra` text COMMENT 'Ingredientes adicionales en formato JSON',
  `precioExtras` decimal(10,2) DEFAULT '0.00',
  `apiProductoId` varchar(36) DEFAULT NULL COMMENT 'ID del producto en la API',
  PRIMARY KEY (`iddetalle`),
  KEY `idx_detalle_venta` (`ventaId`),
  KEY `idx_detalle_producto` (`productoId`),
  CONSTRAINT `detalle_venta_ibfk_1` FOREIGN KEY (`ventaId`) REFERENCES `venta` (`idventa`) ON DELETE CASCADE,
  CONSTRAINT `detalle_venta_ibfk_2` FOREIGN KEY (`productoId`) REFERENCES `producto` (`idproducto`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Detalles de cada venta';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalle_venta`
--

LOCK TABLES `detalle_venta` WRITE;
/*!40000 ALTER TABLE `detalle_venta` DISABLE KEYS */;
/*!40000 ALTER TABLE `detalle_venta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empleado`
--

DROP TABLE IF EXISTS `empleado`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empleado` (
  `idempleado` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `usuario` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `rol` enum('Administrador','Gerente','Cajero','Cocinero') DEFAULT 'Cajero',
  `activo` tinyint(1) DEFAULT '1',
  `fechaIngreso` datetime DEFAULT CURRENT_TIMESTAMP,
  `ultimoAcceso` datetime DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `salario` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`idempleado`),
  UNIQUE KEY `usuario` (`usuario`),
  KEY `idx_empleado_usuario` (`usuario`),
  KEY `idx_empleado_rol` (`rol`),
  KEY `idx_empleado_activo` (`activo`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Tabla de empleados del sistema';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empleado`
--

LOCK TABLES `empleado` WRITE;
/*!40000 ALTER TABLE `empleado` DISABLE KEYS */;
INSERT INTO `empleado` VALUES (1,'Administrador','Sistema','admin','admin123','Administrador',1,'2025-07-13 02:11:37',NULL,NULL,NULL,NULL),(2,'Cajero','Principal','cajero','cajero123','Cajero',1,'2025-07-13 02:11:37',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `empleado` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `factura`
--

DROP TABLE IF EXISTS `factura`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `factura` (
  `idfactura` int NOT NULL AUTO_INCREMENT,
  `fechaFactura` date NOT NULL DEFAULT (curdate()),
  `fkcliente` int NOT NULL,
  `tipo_entrega` enum('Local','Domicilio','Para_Llevar') DEFAULT 'Local',
  `subtotal` decimal(12,2) DEFAULT '0.00',
  `iva` decimal(12,2) DEFAULT '0.00',
  `total` decimal(12,2) DEFAULT '0.00',
  `estado` enum('Pendiente','Pagado','Cancelado') DEFAULT 'Pendiente',
  `empleadoId` int DEFAULT NULL,
  `fechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `notas` text,
  PRIMARY KEY (`idfactura`),
  KEY `empleadoId` (`empleadoId`),
  KEY `idx_factura_fecha` (`fechaFactura`),
  KEY `idx_factura_cliente` (`fkcliente`),
  KEY `idx_factura_estado` (`estado`),
  CONSTRAINT `factura_ibfk_1` FOREIGN KEY (`fkcliente`) REFERENCES `cliente` (`idCliente`) ON DELETE RESTRICT,
  CONSTRAINT `factura_ibfk_2` FOREIGN KEY (`empleadoId`) REFERENCES `empleado` (`idempleado`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Tabla principal de facturas';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `factura`
--

LOCK TABLES `factura` WRITE;
/*!40000 ALTER TABLE `factura` DISABLE KEYS */;
INSERT INTO `factura` VALUES (1,'2025-07-13',1,'Local',0.00,0.00,0.00,'Pendiente',1,'2025-07-13 02:19:36',NULL),(2,'2025-07-13',1,'Domicilio',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 02:21:29',NULL),(3,'2025-07-13',1,'Domicilio',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 02:22:25',NULL),(4,'2025-07-13',1,'Domicilio',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 02:26:00',NULL),(5,'2025-07-13',1,'Local',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 04:16:40',NULL),(6,'2025-07-13',1,'Local',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 04:20:39',NULL),(7,'2025-07-13',1,'Local',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 04:38:30',NULL),(8,'2025-07-13',1,'Local',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 04:40:20',NULL),(9,'2025-07-13',1,'Local',0.00,0.00,0.00,'Pendiente',NULL,'2025-07-13 05:12:01',NULL);
/*!40000 ALTER TABLE `factura` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ingrediente`
--

DROP TABLE IF EXISTS `ingrediente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingrediente` (
  `idingrediente` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `precioExtra` decimal(8,2) DEFAULT '0.00',
  `disponible` tinyint(1) DEFAULT '1',
  `esVegetariano` tinyint(1) DEFAULT '1',
  `categoria` enum('Carnes','Vegetales','Quesos','Salsas','Otros') DEFAULT 'Otros',
  `descripcion` text,
  PRIMARY KEY (`idingrediente`),
  KEY `idx_ingrediente_nombre` (`nombre`),
  KEY `idx_ingrediente_categoria` (`categoria`),
  KEY `idx_ingrediente_disponible` (`disponible`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Tabla de ingredientes disponibles';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingrediente`
--

LOCK TABLES `ingrediente` WRITE;
/*!40000 ALTER TABLE `ingrediente` DISABLE KEYS */;
INSERT INTO `ingrediente` VALUES (1,'Pepperoni',25.00,1,0,'Carnes',NULL),(2,'Jamón',20.00,1,0,'Carnes',NULL),(3,'Salchicha Italiana',30.00,1,0,'Carnes',NULL),(4,'Pollo',25.00,1,0,'Carnes',NULL),(5,'Queso Extra',15.00,1,1,'Quesos',NULL),(6,'Mozzarella Extra',18.00,1,1,'Quesos',NULL),(7,'Queso Cheddar',20.00,1,1,'Quesos',NULL),(8,'Champiñones',12.00,1,1,'Vegetales',NULL),(9,'Pimientos',10.00,1,1,'Vegetales',NULL),(10,'Cebolla',8.00,1,1,'Vegetales',NULL),(11,'Aceitunas Negras',15.00,1,1,'Vegetales',NULL),(12,'Aceitunas Verdes',15.00,1,1,'Vegetales',NULL),(13,'Piña',12.00,1,1,'Vegetales',NULL),(14,'Tomate',10.00,1,1,'Vegetales',NULL),(15,'Salsa BBQ',5.00,1,1,'Salsas',NULL),(16,'Salsa Ranch',5.00,1,1,'Salsas',NULL),(17,'Salsa Picante',5.00,1,1,'Salsas',NULL);
/*!40000 ALTER TABLE `ingrediente` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pos_terminals`
--

DROP TABLE IF EXISTS `pos_terminals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pos_terminals` (
  `id` varchar(36) NOT NULL DEFAULT (uuid()),
  `nombre` varchar(100) NOT NULL,
  `ubicacion` varchar(200) DEFAULT NULL,
  `ultimo_sync` datetime DEFAULT NULL,
  `estado` enum('activo','inactivo','sincronizando') DEFAULT 'activo',
  `fecha_registro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pos_terminals`
--

LOCK TABLES `pos_terminals` WRITE;
/*!40000 ALTER TABLE `pos_terminals` DISABLE KEYS */;
INSERT INTO `pos_terminals` VALUES ('70c290bd-5fb8-11f0-a049-34298f730067','Terminal Principal','Santo Domingo Este',NULL,'activo','2025-07-13 03:10:21');
/*!40000 ALTER TABLE `pos_terminals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `producto`
--

DROP TABLE IF EXISTS `producto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `producto` (
  `idproducto` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(200) NOT NULL,
  `precioProducto` decimal(10,2) NOT NULL,
  `stock` int NOT NULL DEFAULT '0',
  `descripcion` text,
  `tipo` enum('Pizza','Bebida','Postre','Entrada','Complemento') DEFAULT 'Complemento',
  `disponible` tinyint(1) DEFAULT '1',
  `fechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fechaActualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `apiId` varchar(36) DEFAULT NULL COMMENT 'ID para sincronización con API',
  `sincronizadoConAPI` tinyint(1) DEFAULT '0',
  `imagen` varchar(255) DEFAULT NULL,
  `ingredientes` text COMMENT 'Lista de ingredientes principal',
  `tiempoPreparacion` int DEFAULT '15' COMMENT 'Tiempo en minutos',
  PRIMARY KEY (`idproducto`),
  KEY `idx_producto_nombre` (`nombre`),
  KEY `idx_producto_tipo` (`tipo`),
  KEY `idx_producto_disponible` (`disponible`),
  KEY `idx_producto_api` (`apiId`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Tabla de productos/pizzas del sistema POS';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `producto`
--

LOCK TABLES `producto` WRITE;
/*!40000 ALTER TABLE `producto` DISABLE KEYS */;
INSERT INTO `producto` VALUES (1,'Pizza Margherita',450.00,47,'Pizza clásica con salsa de tomate, mozzarella y albahaca fresca','Pizza',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabb7002-6007-11f0-a049-34298f730067',0,NULL,'Salsa de tomate, Mozzarella, Albahaca',15),(2,'Pizza Pepperoni',520.00,50,'Pizza con pepperoni y mozzarella','Pizza',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd6fcf-6007-11f0-a049-34298f730067',0,NULL,'Salsa de tomate, Mozzarella, Pepperoni',15),(3,'Pizza Hawaiana',480.00,50,'Pizza con jamón y piña','Pizza',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd75e5-6007-11f0-a049-34298f730067',0,NULL,'Salsa de tomate, Mozzarella, Jamón, Piña',15),(4,'Pizza Vegetariana',460.00,50,'Pizza con vegetales frescos','Pizza',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd78bf-6007-11f0-a049-34298f730067',0,NULL,'Salsa de tomate, Mozzarella, Pimientos, Cebolla, Champiñones',15),(5,'Pizza Cuatro Quesos',580.00,29,'Pizza con cuatro tipos de quesos','Pizza',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd7b29-6007-11f0-a049-34298f730067',0,NULL,'Salsa blanca, Mozzarella, Cheddar, Parmesano, Ricotta',15),(6,'Pizza Suprema',650.00,30,'Pizza con todos los ingredientes','Pizza',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd7d13-6007-11f0-a049-34298f730067',0,NULL,'Salsa de tomate, Mozzarella, Pepperoni, Jamón, Pimientos, Cebolla, Champiñones',15),(7,'Coca Cola 355ml',60.00,99,'Refresco de cola','Bebida',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd8041-6007-11f0-a049-34298f730067',0,NULL,'',15),(8,'Pepsi 355ml',60.00,96,'Refresco de cola','Bebida',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd844d-6007-11f0-a049-34298f730067',0,NULL,'',15),(9,'Sprite 355ml',60.00,100,'Refresco de limón','Bebida',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd8609-6007-11f0-a049-34298f730067',0,NULL,'',15),(10,'Agua Purificada 500ml',35.00,200,'Agua purificada','Bebida',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd8750-6007-11f0-a049-34298f730067',0,NULL,'',15),(11,'Jugo de Naranja Natural',80.00,50,'Jugo natural de naranja','Bebida',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd889a-6007-11f0-a049-34298f730067',0,NULL,'',15),(12,'Tiramisu',180.00,20,'Postre italiano con café','Postre',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd8a08-6007-11f0-a049-34298f730067',0,NULL,'',15),(13,'Cheesecake',150.00,25,'Pastel de queso con fresas','Postre',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd8b6d-6007-11f0-a049-34298f730067',0,NULL,'',15),(14,'Helado Vainilla',120.00,30,'Helado artesanal de vainilla','Postre',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabd8f46-6007-11f0-a049-34298f730067',0,NULL,'',15),(15,'Pan de Ajo',120.00,40,'Pan tostado con mantequilla de ajo','Entrada',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabda4d9-6007-11f0-a049-34298f730067',0,NULL,'',15),(16,'Alitas BBQ',280.00,30,'Alitas de pollo con salsa BBQ','Entrada',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabda773-6007-11f0-a049-34298f730067',0,NULL,'',15),(17,'Dedos de Mozzarella',220.00,25,'Bastones de mozzarella empanizados','Entrada',1,'2025-07-13 02:11:37','2025-07-13 12:39:16','eabda922-6007-11f0-a049-34298f730067',0,NULL,'',15),(18,'Prueba',1.00,190,NULL,'Complemento',1,'2025-07-13 04:39:50','2025-07-13 12:39:16','eabdab0a-6007-11f0-a049-34298f730067',0,NULL,NULL,15);
/*!40000 ALTER TABLE `producto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sync_queue`
--

DROP TABLE IF EXISTS `sync_queue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sync_queue` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tipo_operacion` enum('venta','producto','cliente') NOT NULL,
  `datos_json` text NOT NULL,
  `intentos` int DEFAULT '0',
  `max_intentos` int DEFAULT '3',
  `proximo_intento` datetime DEFAULT NULL,
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `estado` enum('pendiente','procesando','completado','fallido') DEFAULT 'pendiente',
  `error_ultimo` text,
  `prioridad` int DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sync_queue`
--

LOCK TABLES `sync_queue` WRITE;
/*!40000 ALTER TABLE `sync_queue` DISABLE KEYS */;
INSERT INTO `sync_queue` VALUES (1,'venta','{\"ventaId\":0,\"timestamp\":\"2025-07-13T04:17:08.3055244-04:00\"}',0,3,NULL,'2025-07-13 04:17:08','pendiente',NULL,1),(2,'venta','{\"ventaId\":0,\"timestamp\":\"2025-07-13T04:20:41.4338222-04:00\"}',0,3,NULL,'2025-07-13 04:20:41','pendiente',NULL,1),(3,'venta','{\"ventaId\":0,\"timestamp\":\"2025-07-13T04:38:33.7218787-04:00\"}',0,3,NULL,'2025-07-13 04:38:33','pendiente',NULL,1),(4,'venta','{\"ventaId\":0,\"timestamp\":\"2025-07-13T04:40:23.6734383-04:00\"}',0,3,NULL,'2025-07-13 04:40:23','pendiente',NULL,1),(5,'venta','{\"ventaId\":0,\"timestamp\":\"2025-07-13T05:12:03.0205053-04:00\"}',0,3,NULL,'2025-07-13 05:12:03','pendiente',NULL,1);
/*!40000 ALTER TABLE `sync_queue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `v_sync_pendientes`
--

DROP TABLE IF EXISTS `v_sync_pendientes`;
/*!50001 DROP VIEW IF EXISTS `v_sync_pendientes`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `v_sync_pendientes` AS SELECT 
 1 AS `id`,
 1 AS `tipo_operacion`,
 1 AS `estado`,
 1 AS `intentos`,
 1 AS `fecha_creacion`,
 1 AS `error_ultimo`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `venta`
--

DROP TABLE IF EXISTS `venta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `venta` (
  `idventa` int NOT NULL AUTO_INCREMENT,
  `numeroOrden` varchar(50) NOT NULL,
  `clienteId` int DEFAULT NULL,
  `empleadoId` int NOT NULL,
  `fechaVenta` datetime DEFAULT CURRENT_TIMESTAMP,
  `subtotal` decimal(12,2) NOT NULL DEFAULT '0.00',
  `iva` decimal(12,2) DEFAULT '0.00' COMMENT 'IVA 18%',
  `descuento` decimal(12,2) DEFAULT '0.00',
  `total` decimal(12,2) NOT NULL,
  `metodoPago` enum('Efectivo','Tarjeta_Debito','Tarjeta_Credito','Transferencia') DEFAULT 'Efectivo',
  `estado` enum('Borrador','Pendiente','En_Proceso','Listo','Entregado','Cancelado') DEFAULT 'Pendiente',
  `tipoOrden` enum('Local','Para_Llevar','Domicilio') DEFAULT 'Local',
  `notas` text,
  `direccionEntrega` text,
  `apiPedidoId` varchar(36) DEFAULT NULL COMMENT 'ID para sincronización con API',
  `sincronizadoConAPI` tinyint(1) DEFAULT '0',
  `sync_intentos` int DEFAULT '0',
  `sync_ultimo_intento` datetime DEFAULT NULL,
  `sync_error` text,
  `sync_completado` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`idventa`),
  UNIQUE KEY `numeroOrden` (`numeroOrden`),
  KEY `idx_venta_fecha` (`fechaVenta`),
  KEY `idx_venta_estado` (`estado`),
  KEY `idx_venta_empleado` (`empleadoId`),
  KEY `idx_venta_cliente` (`clienteId`),
  KEY `idx_venta_numero` (`numeroOrden`),
  CONSTRAINT `venta_ibfk_1` FOREIGN KEY (`clienteId`) REFERENCES `cliente` (`idCliente`) ON DELETE SET NULL,
  CONSTRAINT `venta_ibfk_2` FOREIGN KEY (`empleadoId`) REFERENCES `empleado` (`idempleado`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Tabla principal de ventas';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `venta`
--

LOCK TABLES `venta` WRITE;
/*!40000 ALTER TABLE `venta` DISABLE KEYS */;
/*!40000 ALTER TABLE `venta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Final view structure for view `v_sync_pendientes`
--

/*!50001 DROP VIEW IF EXISTS `v_sync_pendientes`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `v_sync_pendientes` AS select `sq`.`id` AS `id`,`sq`.`tipo_operacion` AS `tipo_operacion`,`sq`.`estado` AS `estado`,`sq`.`intentos` AS `intentos`,`sq`.`fecha_creacion` AS `fecha_creacion`,`sq`.`error_ultimo` AS `error_ultimo` from `sync_queue` `sq` where (`sq`.`estado` in ('pendiente','procesando')) order by `sq`.`prioridad` desc,`sq`.`fecha_creacion` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-13 12:51:48
