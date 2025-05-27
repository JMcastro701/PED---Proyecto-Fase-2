CREATE DATABASE HospitalAtencion;
GO

USE HospitalAtencion;
GO


CREATE TABLE Pacientes (
    DUI VARCHAR(20) PRIMARY KEY,
    Nombres VARCHAR(50) NOT NULL,
    Apellidos VARCHAR(50) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Sexo VARCHAR(10),
    Direccion VARCHAR(255),
    Telefono INT,
    Correo VARCHAR(100),
    Peso DECIMAL(5,2),
    Altura DECIMAL(5,2),
    FechaRegistro DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE)); -- Se guardara unicamente en la bd
GO

CREATE TABLE Turnos (
    CodigoTurno INT PRIMARY KEY,
    DUI VARCHAR(20) NOT NULL,
    Prioridad INT NOT NULL, 
    EstadoTurno BIT NOT NULL DEFAULT 0, -- 0 = No atendido, 1 = Atendido
	FechaTurno DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
	HoraTurno TIME NOT NULL DEFAULT CAST(GETDATE() AS TIME),

    FOREIGN KEY (DUI) REFERENCES Pacientes(DUI));
GO







CREATE TABLE Usuarios(
	UsuarioID  INT PRIMARY KEY,
	Nombre_Usuario VARCHAR(50),
    Nombre_Rol VARCHAR(50),
	Correo  VARCHAR(255) UNIQUE NOT NULL,
	Contraseña VARCHAR(50),
	Telefono VARCHAR(20)
);
GO



CREATE TABLE Especialidades (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Especialidad NVARCHAR(100) NOT NULL);
GO

CREATE TABLE Personal (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Especialidad NVARCHAR(100) NOT NULL);
GO
--CREATE TABLE Personal (
--    Id INT PRIMARY KEY IDENTITY(1,1),
--    Nombre NVARCHAR(100) NOT NULL,
--    Especialidad INT NOT NULL,
--    FOREIGN KEY (Especialidad) REFERENCES Especialidades(Id));


	GO
CREATE TABLE HistorialMedico (
	NumeroReporte INT PRIMARY KEY,
    DUI VARCHAR(20) NOT NULL,
    Fecha DATE NOT NULL,
    Diagnostico VARCHAR(MAX),
    Tratamiento VARCHAR(MAX),
    TipoReporte VARCHAR(50),
    Codigo_Medico INT,
	FOREIGN KEY (Codigo_Medico) REFERENCES Personal(Id),

    FOREIGN KEY (DUI) REFERENCES Pacientes(DUI)); 
GO
CREATE TABLE PacientesMedicos (
	ID_Asignacion_de_Medico INT PRIMARY KEY,
    DUI_Paciente VARCHAR(20) NOT NULL,
    Id_Medico INT NOT NULL,
    FechaAsignacion DATE NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (DUI_Paciente) REFERENCES Pacientes(DUI),
    FOREIGN KEY (Id_Medico) REFERENCES Personal(Id));

	GO

-- REGISTROS DE DATOS TABLA: Pacientes
INSERT INTO Pacientes (DUI, Nombres, Apellidos, FechaNacimiento, Sexo, Direccion, Telefono, Correo, Peso, Altura, FechaRegistro)
VALUES
('01234567-8', 'Carlos Alberto', 'Ramírez López', '1990-05-12', 'Hombre', 'Colonia Escalón, San Salvador', 78451230, 'carlos.ramirez@example.com', 165.50, 1.75, CAST(GETDATE() AS DATE)),
('12345678-9', 'Ana María', 'González Torres', '1985-03-22', 'Mujer', 'Santa Tecla, La Libertad', 77233456, 'ana.gonzalez@example.com', 132.40, 1.62, '2022-06-01'),
('23456789-0', 'Luis Fernando', 'Mendoza Pérez', '1992-11-10', 'Hombre', 'Soyapango, San Salvador', 78904567, 'luis.mendoza@example.com', 180.75, 1.80, '2020-07-01'),
('34567890-1', 'María José', 'Cruz Hernández', '1998-07-05', 'Mujer', 'Mejicanos, San Salvador', 76543210, 'maria.cruz@example.com', 145.20, 1.68, '2024-12-01'),
('45678901-2', 'José Antonio', 'Martínez Díaz', '1980-12-01', 'Hombre', 'San Miguel, San Miguel', 77234567, 'jose.martinez@example.com', 200.60, 1.78, '2019-04-21'),
('56789012-3', 'Claudia Patricia', 'Castillo Romero', '1995-09-30', 'Mujer', 'Usulután, Usulután', 78123456, 'claudia.castillo@example.com', 155.10, 1.65, '2020-10-18'),
('67890123-4', 'Ricardo Javier', 'López Méndez', '1988-02-14', 'Hombre', 'Ahuachapán, Ahuachapán', 79012345, 'ricardo.lopez@example.com', 172.80, 1.72, CAST(GETDATE() AS DATE)),
('78901234-5', 'Gabriela Isabel', 'Chávez Morán', '1993-06-18', 'Mujer', 'Santa Ana, Santa Ana', 78345678, 'gabriela.chavez@example.com', 138.00, 1.63, '2021-09-15'),
('89012345-6', 'Fernando Enrique', 'Salazar Quintanilla', '1982-04-25', 'Hombre', 'Ilopango, San Salvador', 77654321, 'fernando.salazar@example.com', 190.25, 1.76,'2018-03-24'),
('90123456-7', 'Jessica Carolina', 'Vásquez Carrillo', '1999-10-08', 'Mujer', 'Zacatecoluca, La Paz', 77553344, 'jessica.vasquez@example.com', 142.60, 1.60, '2023-08-30');

-- REGISTROS DE DATOS TABLA: Turnos
INSERT INTO Turnos (CodigoTurno, DUI, Prioridad, EstadoTurno, FechaTurno) 
VALUES 
(1, '01234567-8', 3, 1, '2025-06-01'),  -- Atendido
(2, '12345678-9', 2, 0, '2025-06-04'),  -- No atendido
(3, '23456789-0', 3, 1, CAST(GETDATE() AS DATE)),  -- Atendido hoy
(4, '34567890-1', 2, 0, '2025-08-20'),  -- No atendido
(5, '45678901-2', 2, 0, '2025-09-18'),  -- No atendido
(6, '56789012-3', 1, 1, CAST(GETDATE() AS DATE)),  -- Atendido hoy
(7, '67890123-4', 1, 0, '2025-08-02'),  -- No atendido
(8, '78901234-5', 3, 1, CAST(GETDATE() AS DATE)),  -- Atendido hoy
(9, '89012345-6', 2, 0, '2025-05-30'),  -- No atendido
(10, '90123456-7', 3, 1, CAST(GETDATE() AS DATE));  -- Atendido hoy 




-- REGISTROS DE DATOS TABLA: Especialidades
INSERT INTO Especialidades (Especialidad)
VALUES
(N'Medicina General'), 
(N'Pediatría'), 
(N'Geriatría'),
(N'Medicina Interna'), 
(N'Cardiología'), 
(N'Neumología'),
(N'Neurología'),
(N'Cirugía General'), 
(N'Cirugía Cardiotorácica'), 
(N'Cirugía Pediátrica'),
(N'Maternidad'); 

-- REGISTROS DE DATOS TABLA: PersonalMedico
INSERT INTO Personal (Nombre,Especialidad)
VALUES
(N'Luis Martínez', 'Medicina General'),
(N'Marcela Torres', 'Pediatría'),
(N'Carlos Reyes',  'Geriatría'),
(N'Verónica López', 'Cardiología'),
(N'José Morales', 'Medicina Interna'),
(N'MaríaHernández', 'Cirugía Cardiotorácica'),
(N'Daniel Sosa',  'Cirugía Pediátrica'),
(N'Lucía Flores', 'Maternidad'),
(N'Elena Cruz',  'Neumología'),
(N'Maria Latoroe', 'Cirugía General');




-- REGISTROS DE DATOS TABLA: HistorialMedico
INSERT INTO HistorialMedico (NumeroReporte, DUI, Fecha, Diagnostico, Tratamiento, TipoReporte, Codigo_Medico) --Para fecha de historial tendria que ser mayor o igual que la fecha de registro de pacientes
VALUES
(1,'01234567-8', '2025-05-12', 'Infección respiratoria', 'Antibióticos y reposo', 'Consulta general', 1),
(2,'12345678-9', '2023-11-22', 'Dolor muscular', 'Analgésicos y fisioterapia', 'Consulta de emergencia',2),
(3,'23456789-0', '2025-03-12', 'Fractura de pierna', 'Yeso y cirugía en caso de complicaciones', 'Reporte quirúrgico',3),
(4,'34567890-1', '2025-02-02', 'Manejo de diabetes', 'Insulina y dieta balanceada', 'Consulta de control',4),
(5,'45678901-2', '2023-01-15', 'Hipertensión', 'Medicamentos antihipertensivos', 'Consulta de seguimiento',5),
(6,'56789012-3', '2022-04-05', 'Alergia estacional', 'Antihistamínicos y evitar alérgenos', 'Consulta general',6),
(7,'67890123-4', '2025-05-12', 'Migraña', 'Analgésicos y descanso', 'Consulta de emergencia',7),
(8,'78901234-5', '2025-02-20', 'Infección urinaria', 'Antibióticos y líquidos', 'Reporte médico',2),
(9,'89012345-6', '2024-10-03', 'Lesión en tobillo', 'Reposo y fisioterapia', 'Consulta de emergencia',3),
(10,'90123456-7', '2024-07-11', 'Fiebre alta', 'Antipiréticos y descanso', 'Consulta médica',4);

-- REGISTROS DE DATOS TABLA: PacientesMedicos
INSERT INTO PacientesMedicos (ID_Asignacion_de_Medico, DUI_Paciente, Id_Medico, FechaAsignacion)
VALUES
(1, '01234567-8', 1, GETDATE()),
(2, '12345678-9', 2, '2023-02-01'),
(3, '23456789-0', 3, '2021-01-05'),
(4, '34567890-1', 4, GETDATE()),
(5, '45678901-2', 5, '2022-10-03'),
(6, '56789012-3', 6, '2022-10-04'),
(7, '67890123-4', 7, GETDATE()), 
(8, '78901234-5', 8, '2021-10-06'),
(9, '89012345-6', 9, '2023-01-06'),
(10, '90123456-7', 10, '2024-07-12'); 

-- REGISTROS DE DATOS TABLA: Usuarios (asumiendo que los TipoRolIDs son los insertados previamente)
INSERT INTO Usuarios (UsuarioID, Nombre_Usuario, Nombre_Rol, Correo, Contraseña, Telefono) 
VALUES 
(1, 'Carlito Tevez', 'Recepcion', 'Carlito.Ruiz@hotmail.com', '123', '73886192'),
(2, 'Marta Gomez','Jefe de especialidad', 'marta.gomez@outlook.com', '456', '70330683'),
(3, 'Luis Lopez','Soporte tecnico', 'luis.lopez@gmail.com', '789', '72693131'), 
(4, 'Ana Torres', 'Director','ana.torres@gmail.com', 'abc', '66778890'),
(5, 'Richard Rios', 'Administrador', 'Richard.Rios@yahoo.com', 'def', '1234567890');







