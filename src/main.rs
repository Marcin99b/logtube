use tokio::net::{TcpListener, TcpStream};
use std::{fs, io::Write};

#[tokio::main]
 async fn main() {
     let listener = TcpListener::bind("127.0.0.1:8080").await.unwrap(); 

    let mut file = fs::File::create("logs.log").unwrap();
     loop {
         let (stream, _) = listener.accept().await.unwrap(); 
         stream.readable().await.unwrap();
         let mut buf = read_all(&stream).await.unwrap();
         buf.push(10); //\n
         let _ = file.write(&buf);
         //let string = String::from_utf8(buf).unwrap();
         //println!("{}", string);
     }
 }

 async fn read_all(stream: &TcpStream) -> Result<Vec<u8>, std::io::Error> {
     let mut buf: Vec<u8> = Vec::new();

     loop {
         let mut tmp_buf = [0; 4096];
         match stream.try_read(&mut tmp_buf) {
             Ok(0) => break,
             Ok(_) => {
                 buf.extend_from_slice(&tmp_buf)
             }
             Err(ref e) if e.kind() == std::io::ErrorKind::WouldBlock => { 
                 break;
             }
             Err(e) => {
                 return Err(e.into());
             }
         }
     }
     return Ok(std::str::from_utf8(&buf).unwrap().trim_matches(char::from(0)).as_bytes().to_vec())
 }
