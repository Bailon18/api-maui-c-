

CREATE DATABASE marpaitdb;
GO

USE marpaitdb;
GO

CREATE TABLE Roles (
    rol_id INT PRIMARY KEY IDENTITY,
    nombre_rol NVARCHAR(50)
);

CREATE TABLE Departamentos (
    departamento_id INT PRIMARY KEY IDENTITY,
    nombre_departamento NVARCHAR(50)
);



CREATE TABLE Usuarios (
    usuario_id INT PRIMARY KEY IDENTITY,
    correo NVARCHAR(100),
    nombre NVARCHAR(50),
    apellidos NVARCHAR(50),
    contrasena NVARCHAR(100),
	codigoempleado NVARCHAR(5),
    estado NVARCHAR(10) DEFAULT 'activo', 
	fecha_ingreso DATE,
    rol_id INT,
	departamento_id INT,
    FOREIGN KEY (rol_id) REFERENCES Roles(rol_id),
	FOREIGN KEY (departamento_id) REFERENCES Departamentos(departamento_id)
);


CREATE TABLE Solicitudes_Permiso (
    solicitud_id INT PRIMARY KEY IDENTITY,
    usuario_id INT,
    motivo NVARCHAR(MAX),
    estado_aprobacion NVARCHAR(10) DEFAULT 'en_espera',
    cuenta_vacaciones BIT,
    cuenta_dias_laborales BIT,
    goce_sueldo BIT,
    tipo_permiso NVARCHAR(10),
    fecha_solicitud DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

-- Ejemplo con Detalles_Solicitud_Dias
CREATE TABLE Detalles_Solicitud_Dias (
    solicitud_id INT PRIMARY KEY,
    fecha_inicio DATE,
    fecha_fin DATE,
    FOREIGN KEY (solicitud_id) REFERENCES Solicitudes_Permiso(solicitud_id) ON DELETE CASCADE
);

-- Ejemplo con Detalles_Solicitud_Horas
CREATE TABLE Detalles_Solicitud_Horas (
    solicitud_id INT PRIMARY KEY,
    hora_inicio TIME,
    hora_fin TIME,
    FOREIGN KEY (solicitud_id) REFERENCES Solicitudes_Permiso(solicitud_id) ON DELETE CASCADE
);


CREATE TABLE Calendarizacion (
    usuario_id INT PRIMARY KEY,
    dias_totales INT DEFAULT 15,
    dias_tomados INT DEFAULT 0,
    dias_restantes INT DEFAULT 15,
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);



-- insert

-- Inserts en la tabla Roles
INSERT INTO Roles (nombre_rol)
VALUES ('Administrador'),
       ('Supervisor'),
       ('Empleado');

-- Inserts en la tabla Departamentos
INSERT INTO Departamentos (nombre_departamento)
VALUES ('Recursos Humanos'),
       ('Ventas'),
       ('Tecnología');

-- Inserts en la tabla Usuarios
INSERT INTO Usuarios (correo, nombre, apellidos, contrasena, codigoempleado, rol_id, departamento_id)
VALUES ('usuario1@empresa.com', 'Juan', 'López', 'contraseña123', 'EMP01', '2023-01-15', 1, 1),
       ('usuario2@empresa.com', 'María', 'García', 'clave567', 'EMP02', '2022-11-28', 2, 2),
       ('usuario3@empresa.com', 'Luis', 'Martínez', 'acceso2023', 'EMP03', '2023-03-10', 3, 3);

-- Inserts en la tabla Solicitudes_Permiso
INSERT INTO Solicitudes_Permiso (usuario_id, motivo, cuenta_vacaciones, cuenta_dias_laborales, goce_sueldo, tipo_permiso, estado_aprobacion, fecha_solicitud)
VALUES (1, 'Vacaciones anuales', 1, 0, 1, 'Dias', 'pendiente', GETDATE()),
       (2, 'Permiso para evento', 0, 1, 1, 'Dias', 'pendiente', GETDATE()),
       (3, 'Descanso médico', 1, 1, 0, 'Dias', 'pendiente', GETDATE());

-- Inserts en la tabla Detalles_Solicitud_Dias
INSERT INTO Detalles_Solicitud_Dias (solicitud_id, fecha_inicio, fecha_fin)
VALUES (1, '2023-11-20', '2023-11-27'),
       (2, '2023-12-05', '2023-12-06'),
       (3, '2023-11-10', '2023-11-11');
go


-- procedimientos almacenados


CREATE PROCEDURE ValidarCredenciales
    @correo NVARCHAR(100),
    @contrasena NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.usuario_id,
        U.correo,
        U.nombre,
        U.apellidos,
        U.estado,
        U.fecha_ingreso,
        R.nombre_rol AS 'rol',
        D.nombre_departamento AS 'departamento'
    FROM
        Usuarios U
    LEFT JOIN
        Roles R ON U.rol_id = R.rol_id
    LEFT JOIN
        Departamentos D ON U.departamento_id = D.departamento_id
    WHERE
        U.correo = @correo AND U.contrasena = @contrasena AND U.estado = 'activo';
END
go

CREATE PROCEDURE ListarUsuarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.usuario_id,
        U.correo,
        U.nombre,
        U.apellidos,
        U.estado,
        U.fecha_ingreso,
		U.codigoempleado,
        R.nombre_rol, -- Agregar el nombre del rol
        D.nombre_departamento -- Agregar el nombre del departamento
    FROM
        Usuarios U
    LEFT JOIN
        Roles R ON U.rol_id = R.rol_id -- Unir con la tabla de Roles
    LEFT JOIN
        Departamentos D ON U.departamento_id = D.departamento_id -- Unir con la tabla de Departamentos;
END




-- Insert


INSERT INTO Roles (nombre_rol) VALUES ('Administrador');
INSERT INTO Roles (nombre_rol) VALUES ('Empleado');


INSERT INTO Departamentos (nombre_departamento) VALUES ('Departamento Sistema');
INSERT INTO Departamentos (nombre_departamento) VALUES ('Departamento Contabilidad');


INSERT INTO Usuarios (correo, nombre, apellidos, contrasena, codigoempleado, estado, rol_id, departamento_id)
VALUES ('admin@example.com', 'Admin', 'Adminson', 'password123', 'A001', 'activo', 1, 1); -- Administrador

INSERT INTO Usuarios (correo, nombre, apellidos, contrasena, codigoempleado, estado, rol_id, departamento_id)
VALUES ('empleado1@example.com', 'Juan', 'Pérez', 'password123', 'E001', 'activo', 2, 2); -- Empleado

INSERT INTO Usuarios (correo, nombre, apellidos, contrasena, codigoempleado, estado, rol_id, departamento_id)
VALUES ('empleado2@example.com', 'María', 'González', 'password123', 'E002', 'activo', 2, 2); -- Empleado


INSERT INTO Solicitudes_Permiso (usuario_id, motivo, estado_aprobacion, cuenta_vacaciones, cuenta_dias_laborales, goce_sueldo, tipo_permiso, fecha_solicitud)
VALUES
    (1, 'Vacaciones anuales', 'aprobado', 1, 1, 1, 'Dias', GETDATE()),
    (2, 'Permiso por enfermedad', 'en_espera', 0, 1, 0, 'horas', GETDATE()),
    (7, 'Permiso por estudio', 'en_espera', 1, 0, 0, 'Dias', GETDATE()),
    (1, 'Permiso por asuntos personales', 'rechazado', 1, 1, 0, 'Horas', GETDATE());


SELECT SP.solicitud_id, SP.motivo, SP.estado_aprobacion, SP.cuenta_vacaciones, SP.cuenta_dias_laborales, SP.goce_sueldo, SP.tipo_permiso, SP.fecha_solicitud
FROM Solicitudes_Permiso SP
INNER JOIN Usuarios U ON SP.usuario_id = U.usuario_id
WHERE U.departamento_id = (SELECT departamento_id FROM Departamentos WHERE departamento_id = 2); 