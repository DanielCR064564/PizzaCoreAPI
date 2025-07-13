-- Add missing columns to Productos table
USE PizzaCoreDb;
-- Add FechaActualizacion column if it doesn't exist
SET @sql = (
        SELECT IF(
                (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'PizzaCoreDb'
                        AND TABLE_NAME = 'Productos'
                        AND COLUMN_NAME = 'FechaActualizacion'
                ) = 0,
                'ALTER TABLE `Productos` ADD COLUMN `FechaActualizacion` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6)',
                'SELECT 1'
            )
    );
PREPARE stmt
FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- Add FechaCreacion column if it doesn't exist
SET @sql = (
        SELECT IF(
                (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'PizzaCoreDb'
                        AND TABLE_NAME = 'Productos'
                        AND COLUMN_NAME = 'FechaCreacion'
                ) = 0,
                'ALTER TABLE `Productos` ADD COLUMN `FechaCreacion` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6)',
                'SELECT 1'
            )
    );
PREPARE stmt
FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- Add Stock column if it doesn't exist
SET @sql = (
        SELECT IF(
                (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'PizzaCoreDb'
                        AND TABLE_NAME = 'Productos'
                        AND COLUMN_NAME = 'Stock'
                ) = 0,
                'ALTER TABLE `Productos` ADD COLUMN `Stock` int NOT NULL DEFAULT 0',
                'SELECT 1'
            )
    );
PREPARE stmt
FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- Add SincronizadoConAPI column if it doesn't exist
SET @sql = (
        SELECT IF(
                (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'PizzaCoreDb'
                        AND TABLE_NAME = 'Productos'
                        AND COLUMN_NAME = 'SincronizadoConAPI'
                ) = 0,
                'ALTER TABLE `Productos` ADD COLUMN `SincronizadoConAPI` tinyint(1) NOT NULL DEFAULT 0',
                'SELECT 1'
            )
    );
PREPARE stmt
FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- Add Imagen column if it doesn't exist
SET @sql = (
        SELECT IF(
                (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'PizzaCoreDb'
                        AND TABLE_NAME = 'Productos'
                        AND COLUMN_NAME = 'Imagen'
                ) = 0,
                'ALTER TABLE `Productos` ADD COLUMN `Imagen` varchar(255) CHARACTER SET utf8mb4 NULL',
                'SELECT 1'
            )
    );
PREPARE stmt
FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- Add IngredientesTexto column if it doesn't exist
SET @sql = (
        SELECT IF(
                (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'PizzaCoreDb'
                        AND TABLE_NAME = 'Productos'
                        AND COLUMN_NAME = 'IngredientesTexto'
                ) = 0,
                'ALTER TABLE `Productos` ADD COLUMN `IngredientesTexto` text CHARACTER SET utf8mb4 NULL',
                'SELECT 1'
            )
    );
PREPARE stmt
FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- Add TiempoPreparacion column if it doesn't exist
SET @sql = (
        SELECT IF(
                (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'PizzaCoreDb'
                        AND TABLE_NAME = 'Productos'
                        AND COLUMN_NAME = 'TiempoPreparacion'
                ) = 0,
                'ALTER TABLE `Productos` ADD COLUMN `TiempoPreparacion` int NOT NULL DEFAULT 15',
                'SELECT 1'
            )
    );
PREPARE stmt
FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
-- Add trigger for FechaActualizacion automatic update
DROP TRIGGER IF EXISTS `productos_update_timestamp`;
CREATE TRIGGER `productos_update_timestamp` BEFORE
UPDATE ON `Productos` FOR EACH ROW
SET NEW.FechaActualizacion = CURRENT_TIMESTAMP(6);
-- Show final table structure
DESCRIBE Productos;