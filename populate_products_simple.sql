-- First, let's see what columns exist in the produtos table:
DESCRIBE Productos;
-- Let's try to insert products that exist in POS but not in API
-- We'll only use the columns that exist in the API
INSERT INTO Productos (
        Id,
        Nombre,
        Descripcion,
        Precio,
        Tipo,
        Disponible
    )
VALUES (
        'eabb7002-6007-11f0-a049-34298f730067',
        'Pizza Margherita',
        'Pizza clásica con salsa de tomate, mozzarella y albahaca fresca',
        450.00,
        'Pizza',
        1
    ),
    (
        'eabd6fcf-6007-11f0-a049-34298f730067',
        'Pizza Pepperoni',
        'Pizza con pepperoni y mozzarella',
        520.00,
        'Pizza',
        1
    ),
    (
        'eabd75e5-6007-11f0-a049-34298f730067',
        'Pizza Hawaiana',
        'Pizza con jamón y piña',
        480.00,
        'Pizza',
        1
    ),
    (
        'eabd78bf-6007-11f0-a049-34298f730067',
        'Pizza Vegetariana',
        'Pizza con vegetales frescos',
        460.00,
        'Pizza',
        1
    ),
    (
        'eabd7b29-6007-11f0-a049-34298f730067',
        'Pizza Cuatro Quesos',
        'Pizza con cuatro tipos de quesos',
        580.00,
        'Pizza',
        1
    ),
    (
        'eabd7d13-6007-11f0-a049-34298f730067',
        'Pizza Suprema',
        'Pizza con todos los ingredientes',
        650.00,
        'Pizza',
        1
    ),
    (
        'eabd8041-6007-11f0-a049-34298f730067',
        'Coca Cola 355ml',
        'Refresco de cola',
        60.00,
        'Bebida',
        1
    ),
    (
        'eabd844d-6007-11f0-a049-34298f730067',
        'Pepsi 355ml',
        'Refresco de cola',
        60.00,
        'Bebida',
        1
    ),
    (
        'eabd8609-6007-11f0-a049-34298f730067',
        'Sprite 355ml',
        'Refresco de limón',
        60.00,
        'Bebida',
        1
    ),
    (
        'eabd8750-6007-11f0-a049-34298f730067',
        'Agua Purificada 500ml',
        'Agua purificada',
        35.00,
        'Bebida',
        1
    ),
    (
        'eabd889a-6007-11f0-a049-34298f730067',
        'Jugo de Naranja Natural',
        'Jugo natural de naranja',
        80.00,
        'Bebida',
        1
    ),
    (
        'eabd8a08-6007-11f0-a049-34298f730067',
        'Tiramisu',
        'Postre italiano con café',
        180.00,
        'Postre',
        1
    ),
    (
        'eabd8b6d-6007-11f0-a049-34298f730067',
        'Cheesecake',
        'Pastel de queso con fresas',
        150.00,
        'Postre',
        1
    ),
    (
        'eabd8f46-6007-11f0-a049-34298f730067',
        'Helado Vainilla',
        'Helado artesanal de vainilla',
        120.00,
        'Postre',
        1
    ),
    (
        'eabda4d9-6007-11f0-a049-34298f730067',
        'Pan de Ajo',
        'Pan tostado con mantequilla de ajo',
        120.00,
        'Entrada',
        1
    ),
    (
        'eabda773-6007-11f0-a049-34298f730067',
        'Alitas BBQ',
        'Alitas de pollo con salsa BBQ',
        280.00,
        'Entrada',
        1
    ),
    (
        'eabda922-6007-11f0-a049-34298f730067',
        'Dedos de Mozzarella',
        'Bastones de mozzarella empanizados',
        220.00,
        'Entrada',
        1
    ),
    (
        'eabdab0a-6007-11f0-a049-34298f730067',
        'Prueba',
        'Producto de prueba',
        1.00,
        'Complemento',
        1
    );
-- Verify the products were inserted
SELECT COUNT(*) as total_products
FROM Productos;
SELECT *
FROM Productos
ORDER BY Nombre;