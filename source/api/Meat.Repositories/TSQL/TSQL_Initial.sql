USE [MeatNet]
GO

INSERT INTO [dbo].[TiposEmpresas] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('P','PROPIA',1,NULL)
INSERT INTO [dbo].[TiposEmpresas] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('U','USUARIO',1,NULL)

INSERT INTO [dbo].[Empresas]([Id],[CodigoEmpresa],[Nombre],[TipoEmpresaId],[NumeroCuit],[NumeroIngresosBrutos]
			,[NumeroInscripcionRuca],[CodigoActividad],[Activo],[ERP_Codigo],[FechaActualizacion],[FechaBaja])
     VALUES ('6B7402E0-4E53-4D10-BFD9-CAA5FDAAAAF8','1','Lenz S.A.','P','2022545092',NULL,NULL,19,1,NULL,GETDATE(),NULL)
INSERT INTO [dbo].[Empresas]([Id],[CodigoEmpresa],[Nombre],[TipoEmpresaId],[NumeroCuit],[NumeroIngresosBrutos]
			,[NumeroInscripcionRuca],[CodigoActividad],[Activo],[ERP_Codigo],[FechaActualizacion],[FechaBaja])
     VALUES (NEWID(),'77','Usuario 077 S.R.L.','U','3041545077',NULL,NULL,19,1,NULL,GETDATE(),NULL)

INSERT INTO [dbo].[Sucursales] ([Id],[CodigoSucursal],[Nombre],[Erp_Codigo],[EmpresaId],[FechaActualizacion],[Direccion],[CodigoPostal],[Localidad],[Provincia],[Zona],[Pais],[Activo],[FechaBaja])
     VALUES ('D6DBC49F-FE86-4F1A-990D-E8C063DDD879','1000','Sucursal Rosario',NULL,'6B7402E0-4E53-4D10-BFD9-CAA5FDAAAAF8',GETDATE(),'Lord Kelvin 2179','2000','Rosario','Santa Fe',NULL,'ARG',1,NULL)

INSERT INTO [dbo].[Puestos]([Id],[CodigoPuesto],[Nombre],[Erp_Codigo],[SucursalId],[Activo],[FechaActualizacion],[FechaBaja])
     VALUES (NEWID(),'PTP01','Palco Tipificacion Porcina',NULL,'D6DBC49F-FE86-4F1A-990D-E8C063DDD879',1,GETDATE(),NULL)

INSERT INTO [dbo].[Roles] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('Admin','Administrador',1,NULL)
INSERT INTO [dbo].[Roles] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('Abastecimiento','Abastecimiento',1,NULL)
INSERT INTO [dbo].[Roles] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('OperadorFaena','Operador Faena',1,NULL)
INSERT INTO [dbo].[Roles] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('OperadorIngreso','Operador Ingreso',1,NULL)

INSERT INTO [dbo].[Usuarios]([Id],[Nombre],[Apellido],[Email],[RolId],[UserName],[PasswordHash],[FechaActualizacion],[Activo],[EmpresaId],[FechaBaja],[Legajo])
     VALUES ('D9CB18A9-4BB0-4169-BB5C-34F9AFD6F91E','Admin','Administrador','admin@lenz.com','Admin','admin','D033E22AE348AEB5660FC2140AEC35850C4DA997',GETDATE(),1,'6B7402E0-4E53-4D10-BFD9-CAA5FDAAAAF8',NULL,'0001')

INSERT INTO [dbo].[UsuariosSucursales] ([Id],[UsuarioId],[SucursalId],[FechaActualizacion],[EsMain],[FechaBaja])
     VALUES (NEWID(),'D9CB18A9-4BB0-4169-BB5C-34F9AFD6F91E','D6DBC49F-FE86-4F1A-990D-E8C063DDD879',GETDATE(),1 ,NULL)

INSERT INTO [dbo].[Especies] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('P','PORCINO',1,NULL)
INSERT INTO [dbo].[Especies] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('V','VACUNO',1,NULL)

INSERT INTO [dbo].[Establecimientos] ([Id],[CodigoEstablecimiento],[Nombre],[SucursalId],[EspecieId],[NumeroSenasa],[NumeroOncca],[Activo],[FechaActualizacion],[FechaBaja])
     VALUES (NEWID(),'1000','Establecimiento Frigorífico 1000 Suc 10','D6DBC49F-FE86-4F1A-990D-E8C063DDD879','P',NULL,NULL,1,GETDATE(),NULL)

INSERT INTO [dbo].[TiposSexos] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('H','HEMBRA',1,NULL)
INSERT INTO [dbo].[TiposSexos] ([Codigo],[Nombre],[Activo],[FechaBaja]) VALUES ('M','MACHO',1,NULL)

INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('CA','CAPONES','P','H','900063',NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('CH','CHANCHAS','P','H','900066',NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('LL','LECHONES LIVIANOS','P','M',NULL,NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('LP','LECHONES PESADOS','P','M',NULL,NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('PA','PADRILLOS','P','M',NULL,NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('NT','NOVILLITO','V','M',NULL,NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('NO','NOVILLO','V','M',NULL,NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('TO','TORO','V','M',NULL,NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('VQ','VAQUILLONA','V','H',NULL,NULL,1,GETDATE(),NULL)
INSERT INTO [dbo].[TiposEspecies] ([Id],[Nombre],[EspecieId],[TipoSexoId],[CodigoMaterialDesde],[CodigoMaterialHasta],[Activo],[FechaActualizacion],[FechaBaja]) VALUES ('VA','VACA','V','H',NULL,NULL,1,GETDATE(),NULL)

GO


