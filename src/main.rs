use axum::{extract::Query, response::IntoResponse, routing::get, Json, Router};
use serde::Deserialize;
use std::{
    fs,
    io::{Read, Write},
};
use tokio::net::{TcpListener, TcpStream};

#[tokio::main]
async fn main() {
    tokio::join!(listen_tcp(), listen_http());
}

async fn listen_tcp() {
    let listener = TcpListener::bind("127.0.0.1:8080").await.unwrap();
    let mut file = fs::File::create("logs.log").unwrap();
    loop {
        let (stream, _) = listener.accept().await.unwrap();
        stream.readable().await.unwrap();
        let mut buf = read_all(&stream).await.unwrap();

        //\r\n is only at end of log
        buf.remove(13); //\r
        buf.push(13); //\n
        buf.push(10); //\n

        let _ = file.write(&buf);
    }
}

async fn listen_http() {
    let listener = TcpListener::bind("127.0.0.1:8081").await.unwrap();
    let app = Router::new().route("/search", get(search));
    axum::serve(listener, app).await.unwrap();
}

async fn read_all(stream: &TcpStream) -> Result<Vec<u8>, std::io::Error> {
    let mut buf: Vec<u8> = Vec::new();

    loop {
        let mut tmp_buf = [0; 4096];
        match stream.try_read(&mut tmp_buf) {
            Ok(0) => break,
            Ok(_) => buf.extend_from_slice(&tmp_buf),
            Err(ref e) if e.kind() == std::io::ErrorKind::WouldBlock => {
                break;
            }
            Err(e) => {
                return Err(e.into());
            }
        }
    }
    return Ok(std::str::from_utf8(&buf)
        .unwrap()
        .trim_matches(char::from(0))
        .as_bytes()
        .to_vec());
}

#[derive(Deserialize)]
struct SearchQuery {
    phrase: String,
}

async fn search(query: Query<SearchQuery>) -> impl IntoResponse {
    //let search_bytes = query.phrase.as_bytes();

    let mut file = fs::File::create("logs.log").unwrap();
    let mut tmp_buf = [0; 4096];
    let size = file.read(&mut tmp_buf).unwrap();
    Json(size)
    //todo find search in file
}

//todo add unique newline
//for example change all \r\n to \n and use \r\n as unique newline
