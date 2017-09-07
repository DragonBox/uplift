task :test do
    pwd = Dir.pwd
    sh "u3d run -- -runTests -projectPath #{pwd} -testResults #{pwd}/results.xml -testPlatform editmode"
end

task :build do
    require 'u3d'
    require 'u3d/installer'
    require 'u3d/unity_project'
    require 'shellwords'
    
    version = U3d::UnityProject.new(Dir.pwd).editor_version
    unity = U3d::Installer.create.installed.find { |u| u.version == version }
    U3dCore::UI.user_error!("Missing version #{version}") if unity.nil?

    # Note: the location of the managed files and mcs has changed between 5.3 and 5.6
    # This task is designed to build DLL with Unity version 5.6+
    if U3d::Helper.mac?
        managed_path = File.join(unity.path, 'Contents', 'Managed')
        mcs_exe = File.join(unity.path, 'Contents', 'MonoBleedingEdge', 'bin', 'mcs')
    elsif U3d::Helper.windows?
        managed_path = File.join(unity.path, 'Editor', 'Data', 'Managed')
        mcs_exe = File.join(unity.path, 'Editor', 'Data', 'MonoBleedingEdge', 'bin', 'mcs')
    else
        raise RuntimeError, "This task does not work on Linux"
    end
    
    references = [
        File.join(managed_path, 'UnityEditor.dll'),
        File.join(managed_path, 'UnityEngine.dll'),
        File.join('Assets', 'Plugins', 'Editor', 'SharpCompress.dll')
    ]

    reference_string = references.map { |dep| dep.shellescape }.join(',')
    files = Dir[ File.join('Assets', 'Plugins', 'Editor', 'Uplift', '**', '*.cs') ].reject { |p| File.directory? p }.reject { |p| /Testing/ =~ p }.join(' ')
    
    Dir.mkdir 'target' unless Dir.exist? 'target'

    sh "#{mcs_exe.shellescape} -r:#{reference_string} -target:library -sdk:2 -out:target/Uplift.dll #{files}"
end