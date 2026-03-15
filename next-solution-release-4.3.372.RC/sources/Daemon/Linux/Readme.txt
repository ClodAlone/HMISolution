*** PASSI PER RICREARE IL DOCK-CONTAINER 'emerson-progea' ***

1. Entrare nella cartella "/home/pacedge/"
2. Eseguire il comando "docker-compose down" (immagine 1.png).
-- (questo non più necessario con modifiche fatte nell'immagine) 3. Eseguire il comando "docker image rm emersonedgestack_progea:latest" (immagine 2.png). 
4. Modificare i files del DeployServer che sono presenti nella cartella "/home/pacedge/progea/DeployServer-Files/"
(per copiare occorre creare una cartella condivisa seguendo il seguente documento https://gist.github.com/estorgio/0c76e29c0439e683caca694f338d4003)
5. Eseguire il comando "docker-compose up -d" (immagine 3.png).
(questa operazione richiederà diversi minuti)
6. Al termine i dock-containers dovrebbero avviarsi automaticamente.

Nota:
Il file "/home/pacedge/docker-compose-yml" è il file di configurazione usato per creare i vari dock-containers.
Usando il comando "nano docker-compose-yml" è possibile entrare in editazione e modificare la parte relativa al DeployServer legata a traefik.
Scommentando la sezione "ports" e commentando tutta la sezione "labels" è possibile disabilitare "traefik" per il "DeployServer".

*** PASSI PER INSTALARE LA LIBRERIA DI LICENZA DEAMON ***

Therefore lets continue with the library installation:
1.	Copy the archive I provided into the folder /home/admin/pacedge/emerson-progea
2.	Login on the VM
3.	Change to the folder: cd /home/admin/pacedge/emerson-progea
4.	Unpack archive: tar xzvf libipld.so.0.5.1.tgz
5.	Execute command (bugfix): sudo chown www-data:www-data /home/admin/pacedge/emerson-nginx/www/emerson/eula-accepted.txt
6.	Login on Progea container: docker exec -it emerson-progea /bin/bash
7.	Change to /progea folder (in container): cd /progea
8.	Copy library to /usr/lib: cp libipld.so.0.5.1 /usr/lib
9.	Link library: ln -s /usr/lib/libipld.so.0.5.1 /usr/lib/libipld.so
10.	Execute ipld command (should list library version 0.51 and some debug output) : ./ipld
11.	Logout from container: (keys) CTRL-D

*** PASSI PER AGGIORNARE LA LIBRERIA DI LICENZA DEAMON ***

Pleas install the library as described in one of my previous mails:
1.	Copy the archive into the folder /home/admin/pacedge/emerson-progea
2.	Login on the VM
3.	Change to the folder: cd /home/admin/pacedge/emerson-progea
4.	Unpack archive: tar xzvf libipld.so.0.5.2.tgz
5.	( Execute command (bugfix): sudo chown www-data:www-data /home/admin/pacedge/emerson-nginx/www/emerson/eula-accepted.txt )
6.	Login on Progea container: docker exec -it emerson-progea /bin/bash
7.	Change to /progea folder (in container): cd /progea
8.	Remove old library: rm /usr/lib/libipld.so.0.5.1
9.	Remove old link:  rm /usr/lib/libipld.so
10.	Copy library to /usr/lib: cp libipld.so.0.5.2 /usr/lib
11.	Link library: ln -s /usr/lib/libipld.so.0.5.2 /usr/lib/libipld.so
12.	Execute ipld command (should list library version 0.52 and some debug output) : ./ipld
13.	Logout from container: (keys) CTRL-D

*** PASSI PER OTTENERE UNA LICENZA DEAMON ***

To get a valid license you need to change the MAC address of the VM (license daemon only accepts a certain range of MACs):
1.	In VirtualBox Manger klick on „Change->Network->Extended“
2.	Change the first 3 Bytes of the MAC to: 7CCBE2
3.	Reboot VM
4.	Call PACEdge Web page in a Web browser: Enter url: http://<IP address of VM>
5.	If required accept EULA and change password (or return to start page)
6.	You will see the start page and a hint that your license is invalid in the top line. The topline also displays the PACEdge ID (see picture below)
7.	Please send me the PACEdge ID. I will return you the license files and instructions where to install

To Install the provided license:
1. 	Please copy the content of the archive to the directory /home/admin/pacedge/emerson-software/license in your VM and reboot the VM.
