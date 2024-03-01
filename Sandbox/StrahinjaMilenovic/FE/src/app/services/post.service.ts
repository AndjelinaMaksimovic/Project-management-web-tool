import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PostService {

  constructor() { }

  async getAllPosts(){
    const res = await (await fetch("http://localhost:8080/posts/all")).json();
    return res;
  }

  async getPost(id: string){
    const res = await (await fetch(`http://localhost:8080/posts?id=${id}`)).json();
    return res;
  }
  async deletePost(id: string){
    const res = (await fetch(`http://localhost:8080/posts/delete?id=${id}`))
    return res;
  }
  async updatePost(id: string, text: string){
    const res = (await fetch(`http://localhost:8080/posts/update?id=${id}&text=${text}`))
    return res;
  }
  async createPost(text: string){
    const res = (await fetch(`http://localhost:8080/posts/create?text=${text}`))
    return res;
  }
}
