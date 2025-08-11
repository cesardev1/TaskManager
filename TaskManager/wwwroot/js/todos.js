function addNewTask(){
    todoListViewModel.todos.push(new todoitemListViewModelFn({id:0,title:''}))
    $("[name=title-todo]").last().focus();
}

function managerFocusoutTitleTodo(todo){
    console.log("Ingresa en funcion");
    const title = todo.title();
    if(!title){
        console.log("Ingresa en if");
        todoListViewModel.todos.pop();
        return;
    }
    console.log("Pasa if sin iniciar");
    todo.id(1);
}