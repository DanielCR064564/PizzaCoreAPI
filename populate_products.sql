-- Script to populate PizzaCoreAPI database with products from POS
-- Run this against the pizzacoredb database
USE pizzacoredb;
-- Insert products that exist in POS but not in API
INSERT INTO productos (
        Id,
        Nombre,
        Descripcion,
        Precio,
        Tipo,
        Disponible,
        PedidoId,
        FechaActualizacion
    )
VALUES -- Products from POS database
    (
        'eabb7002-6007-11f0-a049-34298f730067',
        'Pizza Margherita',
        'Pizza clásica con salsa de tomate, mozzarella y albahaca fresca',
        450.00,
        'Pizza',
        1,
        NULL,
        NOW()
    ),
    (
        'eabd6fcf-6007-11f0-a049-34298f730067',
        'Pizza Pepperoni',
        'Pizza con pepperoni y mozzarella',
        520.00,
        'Pizza',
        1,
        NULL,
        NOW()
    ),
    (
        'eabdf3f2-6007-11f0-a049-34298f730067',
        'Pizza Hawaiana',
        'Pizza con jamón y piña',
        480.00,
        'Pizza',
        1,
        NULL,
        NOW()
    ),
    (
        'eabe0e5e-6007-11f0-a049-34298f730067',
        'Pizza Vegetariana',
        'Pizza con vegetales frescos',
        470.00,
        'Pizza',
        1,
        NULL,
        NOW()
    ),
    (
        'eabe2741-6007-11f0-a049-34298f730067',
        'Pizza Suprema',
        'Pizza con todos los ingredientes',
        580.00,
        'Pizza',
        1,
        NULL,
        NOW()
    ),
    (
        'eabe3e24-6007-11f0-a049-34298f730067',
        'Pizza BBQ',
        'Pizza con salsa BBQ y pollo',
        550.00,
        'Pizza',
        1,
        NULL,
        NOW()
    ),
    (
        'eabe5324-6007-11f0-a049-34298f730067',
        'Coca Cola 355ml',
        'Refresco de cola',
        60.00,
        'Bebida',
        1,
        NULL,
        NOW()
    ),
    (
        'eabe683c-6007-11f0-a049-34298f730067',
        'Sprite 355ml',
        'Refresco de limón',
        60.00,
        'Bebida',
        1,
        NULL,
        NOW()
    ),
    (
        'eabe7c84-6007-11f0-a049-34298f730067',
        'Fanta 355ml',
        'Refresco de naranja',
        60.00,
        'Bebida',
        1,
        NULL,
        NOW()
    ),
    (
        'eabe8fe9-6007-11f0-a049-34298f730067',
        'Agua 500ml',
        'Agua embotellada',
        35.00,
        'Bebida',
        1,
        NULL,
        NOW()
    ),
    (
        'eabea335-6007-11f0-a049-34298f730067',
        'Ensalada César',
        'Ensalada fresca con aderezo César',
        280.00,
        'Ensalada',
        1,
        NULL,
        NOW()
    ),
    (
        'eabeb60d-6007-11f0-a049-34298f730067',
        'Ensalada Griega',
        'Ensalada con queso feta y aceitunas',
        320.00,
        'Ensalada',
        1,
        NULL,
        NOW()
    ),
    (
        'eabec90a-6007-11f0-a049-34298f730067',
        'Pasta Alfredo',
        'Pasta en salsa blanca',
        380.00,
        'Pasta',
        1,
        NULL,
        NOW()
    ),
    (
        'eabedb2c-6007-11f0-a049-34298f730067',
        'Pasta Boloñesa',
        'Pasta con salsa de carne',
        420.00,
        'Pasta',
        1,
        NULL,
        NOW()
    ),
    (
        'eabeee23-6007-11f0-a049-34298f730067',
        'Pan de Ajo',
        'Pan tostado con ajo y mantequilla',
        120.00,
        'Entrada',
        1,
        NULL,
        NOW()
    ),
    (
        'eabf00f5-6007-11f0-a049-34298f730067',
        'Alitas BBQ',
        'Alitas de pollo en salsa BBQ',
        380.00,
        'Entrada',
        1,
        NULL,
        NOW()
    ),
    (
        'eabf1375-6007-11f0-a049-34298f730067',
        'Nachos Supremos',
        'Nachos con queso y jalapeños',
        320.00,
        'Entrada',
        1,
        NULL,
        NOW()
    ),
    (
        'eabf25c5-6007-11f0-a049-34298f730067',
        'Helado de Vainilla',
        'Helado artesanal de vainilla',
        150.00,
        'Postre',
        1,
        NULL,
        NOW()
    ),
    (
        'eabf3826-6007-11f0-a049-34298f730067',
        'Servilletas',
        'Servilletas de papel',
        1.00,
        'Complemento',
        1,
        NULL,
        NOW()
    );
-- Verify products were inserted
SELECT COUNT(*) as total_productos
FROM productos;
SELECT Id,
    Nombre,
    Precio,
    Tipo
FROM productos
ORDER BY Tipo,
    Nombre;