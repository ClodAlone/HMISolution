@echo off
setlocal

echo Processing NodeSet Scehma
xsd /classes /n:Opc.Ua.Export UANodeSet.xsd UAVariant.xsd

echo Processing SecuredApplication Scehma
svcutil /dconly /namespace:*,Opc.Ua.Security /out:SecuredApplication.cs SecuredApplication.xsd 

