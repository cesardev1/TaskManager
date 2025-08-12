function addNewTask(){
    todoListViewModel.todos.push(new todoitemListViewModelFn({id:0,title:''}))
    $("[name=title-todo]").last().focus();
}

async function managerFocusoutTitleTodo(todo){
    const title = todo.title();
    if(!title){
        todoListViewModel.todos.pop();
        return;
    }
    
    const data = JSON.stringify(title);
    const response = await fetch(urlTodos, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        const json = await response.json();
        todo.id(json.id);
    }else{
        // show error message
    }
}


async function getTodos(){
    todoListViewModel.loading(true);
    
    const response = await fetch(urlTodos, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(!response.ok){
        return;
    }
    
    const json = await response.json();
    console.log(json);
    todoListViewModel.todos([]);
    
    json.forEach(value => {
        todoListViewModel.todos.push(new todoitemListViewModelFn(value));
    });
    
    todoListViewModel.loading(false);
}