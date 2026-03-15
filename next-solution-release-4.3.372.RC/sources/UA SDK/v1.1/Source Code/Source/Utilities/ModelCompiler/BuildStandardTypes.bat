@echo off
setlocal

..\..\Bin\Opc.Ua.ModelCompiler.exe -d2 ".\Design\StandardTypes.xml" -d2 ".\Design\UA Core Services.xml" -c ".\Design\StandardTypes.csv" -stack "..\..\" -o2 "..\..\Common\Core\Stack\Generated\" -ansic 

copy "..\..\Common\Core\Stack\Generated\Opc.Ua.NodeSet.xml" "..\..\Common\Core\Schema\Opc.Ua.NodeSet.xml"
copy "..\..\Common\Core\Stack\Generated\Opc.Ua.NodeSet2.xml" "..\..\Common\Core\Schema\Opc.Ua.NodeSet2.xml"
copy "..\..\Common\Core\Stack\Generated\Opc.Ua.Services.wsdl" "..\..\Common\Core\Schema\Opc.Ua.Services.wsdl"
copy "..\..\Common\Core\Stack\Generated\Opc.Ua.Endpoints.wsdl" "..\..\Common\Core\Schema\Opc.Ua.Endpoints.wsdl"
copy "..\..\Common\Core\Stack\Generated\Opc.Ua.Types.xsd" "..\..\Common\Core\Schema\Opc.Ua.Types.xsd"
copy "..\..\Common\Core\Stack\Generated\Opc.Ua.Types.bsd" "..\..\Common\Core\Schema\Opc.Ua.Types.bsd"
copy "..\..\Common\Core\Types\Generated\Opc.Ua.StatusCodes.csv" "..\..\Common\Core\Schema\StatusCode.csv"

copy ".\Design\StandardTypes.csv" "..\..\Common\Core\Schema\NodeIds.csv"
copy ".\Design\UA Attributes.csv" "..\..\Common\Core\Schema\AttributeIds.csv"

del "..\..\Common\Core\Stack\Generated\Opc.Ua.NodeSet.xml" 
del "..\..\Common\Core\Stack\Generated\Opc.Ua.NodeSet2.xml"
del "..\..\Common\Core\Stack\Generated\Opc.Ua.Services.wsdl" 
del "..\..\Common\Core\Stack\Generated\Opc.Ua.Endpoints.wsdl" 
del "..\..\Common\Core\Stack\Generated\Opc.Ua.Types.xsd" 
del "..\..\Common\Core\Stack\Generated\Opc.Ua.Types.bsd"
del "..\..\Common\Core\Types\Generated\Opc.Ua.StatusCodes.csv"
